using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Cradle;
using Cradle.StoryFormats.Harlowe;
using System;
using UnityEngine.Windows.Speech;
using FrostweepGames.Plugins.GoogleCloud.TextToSpeech;

namespace Cradle.Players
{
    [ExecuteInEditMode]
    public class TwineTextPlayer : MonoBehaviour
    {
        public Story Story;
        public RectTransform Container;
        public Button LinkTemplate;
        public Text WordTemplate;
        public RectTransform LineBreakTemplate;
        public bool StartStory = true;
        public bool AutoDisplay = true;
        public bool ShowNamedLinks = true;

        static Regex rx_splitText = new Regex(@"(\s+|[^\s]+)");


        private GCTextToSpeech _gcTextToSpeech;
        public AudioSource audioSourceDenis;
        public AudioSource audioSourceMike;
        public AudioSource audioSourceRemy;
        public AudioSource audioSourceZoe;
        public AudioSource audioSourceStefani;
        public AudioSource audioSourceRegina;

        public Text antworttext;
        GameObject AntwortDisplay;

        public String RemyVoice;
        public String MikeVoice;
        public String DenisVoice;
        public String ZoeVoice;
        public String StefaniVoice;
        public String ReginaVoice;

        GameObject Avatar;

        private String Entiretext;//Text in Variable abspeichern, damit auf Leerzeilen verwendet werden können

        //KeywordRecognizer
        private KeywordRecognizer keywordRecognizer;
        private Dictionary<string, StoryLink> actions = new Dictionary<string, StoryLink>();

        //Warte 3 Sekunden bevor Story beginnt
        private float startWait = 5f;



        private LaunchManager launchManager;
        private LoggingManager log;


        void Start()
        {
            Invoke("Initialize", startWait);
            Debug.Log("Warte " + startWait + " Sekunden");

        }


        void Initialize()
        {

            if (!Application.isPlaying)
                return;

            LinkTemplate.gameObject.SetActive(false);
            ((RectTransform)LinkTemplate.transform).SetParent(null);
            LinkTemplate.transform.hideFlags = HideFlags.HideInHierarchy;

            WordTemplate.gameObject.SetActive(false);
            WordTemplate.rectTransform.SetParent(null);
            WordTemplate.rectTransform.hideFlags = HideFlags.HideInHierarchy;

            LineBreakTemplate.gameObject.SetActive(false);
            LineBreakTemplate.SetParent(null);
            LineBreakTemplate.hideFlags = HideFlags.HideInHierarchy;

            if (this.Story == null)
                this.Story = this.GetComponent<Story>();
            if (this.Story == null)
            {
                Debug.LogError("Text player does not have a story to play. Add a story script to the text player game object, or assign the Story variable of the text player.");
                return;
            }

            this.Story.OnPassageEnter += Story_OnPassageEnter;
            this.Story.OnOutput += Story_OnOutput;
            this.Story.OnOutputRemoved += Story_OnOutputRemoved;

            //Keyword Recognizer Init
            actions.Add("init", null); //just to initialize, will be cleared later
            keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
            keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
            keywordRecognizer.Start();

            //EVE
            //launchManager = GameObject.FindGameObjectWithTag("LaunchManager").GetComponent<LaunchManager>();
            //log = launchManager.LoggingManager;
            //int maxDuration = int.Parse(log.GetParameterValue("maxDuration"));
            //bool timePressure = log.GetParameterValue("timePressure").ToLower() == "yes";

            if (StartStory)
                this.Story.Begin();

        }

        void OnDestroy()
        {
            if (!Application.isPlaying)
                return;

            if (this.Story != null)
            {
                this.Story.OnPassageEnter -= Story_OnPassageEnter;
                this.Story.OnOutput -= Story_OnOutput;
            }
        }

        // .....................
        // Clicks

#if UNITY_EDITOR
        void Update()
        {


            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Show content of dictionary in console
                foreach (string key in actions.Keys)
                    Debug.Log("In Dict: " + key);
            }

            if (Application.isPlaying)
                return;

            // In edit mode, disable autoplay on the story if the text player will be starting the story
            if (this.StartStory)
            {
                foreach (Story story in this.GetComponents<Story>())
                    story.AutoPlay = false;
            }

        }
#endif

        public void Clear()
        {
            for (int i = 0; i < Container.childCount; i++)
                GameObject.Destroy(Container.GetChild(i).gameObject);
            Container.DetachChildren();
            actions.Clear();
            antworttext.text = "";
            antworttext.GetComponent<Text>().color = Color.clear;
            Entiretext = "";
        }

        void Story_OnPassageEnter(StoryPassage passage)
        {

            Clear();

        }

        void Story_OnOutput(StoryOutput output)
        {
            if (!this.AutoDisplay)
                return;
            DisplayOutput(output);


        }

        void Story_OnOutputRemoved(StoryOutput outputThatWasRemoved)
        {
            // Remove all elements related to this output
            foreach (var elem in Container.GetComponentsInChildren<TwineTextPlayerElement>()
                .Where(e => e.SourceOutput == outputThatWasRemoved))
            {
                elem.transform.SetParent(null);
                GameObject.Destroy(elem.gameObject);
            }
        }

        public void DisplayOutput(StoryOutput output)
        {
            // Deternine where to place this output in the hierarchy - right after the last UI element associated with the previous output, if exists
            TwineTextPlayerElement last = Container.GetComponentsInChildren<TwineTextPlayerElement>()
                .Where(elem => elem.SourceOutput.Index < output.Index)
                .OrderBy(elem => elem.SourceOutput.Index)
                .LastOrDefault();
            int uiInsertIndex = last == null ? -1 : last.transform.GetSiblingIndex() + 1;

            // Temporary hack to allow other scripts to change the templates based on the output's Style property
            SendMessage("Twine_BeforeDisplayOutput", output, SendMessageOptions.DontRequireReceiver);


            if (output is StoryText)
            {
                var text = (StoryText)output;


                if (!string.IsNullOrEmpty(text.Text))//Prüfen ob Story Leerzeilen beinhaltet.
                {
                    if (text.Text != " ")
                    {
                        Entiretext += text.Text;
                    }
                }



                //###############################################################################################################
                //Google Cloud Text to Speech
                //###############################################################################################################

                GCTextToSpeech _gcTextToSpeech = GCTextToSpeech.Instance;

                _gcTextToSpeech.SynthesizeSuccessEvent += _gcTextToSpeech_SynthesizeSuccessEvent;
                _gcTextToSpeech.SynthesizeFailedEvent += _gcTextToSpeech_SynthesizeFailedEvent;


                void _gcTextToSpeech_SynthesizeFailedEvent(string error)
                {
                    Debug.Log(error);
                }

                //#endregion failed handlers
                //#region sucess handlers   
                _gcTextToSpeech.GetVoices(new GetVoicesRequest()
                {
                    languageCode = "de_DE"
                });

                //Prüfen welches GameObject aktiv ist, um zugeteilte Stimme auszugeben.
                if (Avatar = GameObject.Find("Denis"))
                {
                    _gcTextToSpeech.Synthesize(Entiretext, new VoiceConfig()
                    {
                        gender = Enumerators.SsmlVoiceGender.MALE,
                        languageCode = "de_DE",
                        name = "" + DenisVoice + ""
                    }
                    );
                }

               else if (Avatar = GameObject.Find("Mike"))
                {
                    _gcTextToSpeech.Synthesize(Entiretext, new VoiceConfig()
                    {
                        gender = Enumerators.SsmlVoiceGender.MALE,
                        languageCode = "de_DE",
                        name = "" + MikeVoice + ""
                    }
                    );
                }

                else if (Avatar = GameObject.Find("Remy"))
                {
                    _gcTextToSpeech.Synthesize(Entiretext, new VoiceConfig()
                    {
                        gender = Enumerators.SsmlVoiceGender.MALE,
                        languageCode = "de_DE",
                        name = ""+RemyVoice+""
                    }
                    );
                }

                else if (Avatar = GameObject.Find("Zoe"))
                {
                    _gcTextToSpeech.Synthesize(Entiretext, new VoiceConfig()
                    {
                        gender = Enumerators.SsmlVoiceGender.FEMALE,
                        languageCode = "de_DE",
                        name = "" + ZoeVoice + ""
                    }
                    );
                }

                else if (Avatar = GameObject.Find("Stefani"))
                {
                    _gcTextToSpeech.Synthesize(Entiretext, new VoiceConfig()
                    {
                        gender = Enumerators.SsmlVoiceGender.FEMALE,
                        languageCode = "de_DE",
                        name = "" + StefaniVoice + ""
                    }
                    );
                }

                else if (Avatar = GameObject.Find("Regina"))
                {
                    _gcTextToSpeech.Synthesize(Entiretext, new VoiceConfig()
                    {
                        gender = Enumerators.SsmlVoiceGender.FEMALE,
                        languageCode = "de_DE",
                        name = "" + ReginaVoice + ""
                    }
                    );
                }


                void _gcTextToSpeech_SynthesizeSuccessEvent(PostSynthesizeResponse response)
                {

                    if (Avatar = GameObject.Find("Denis"))
                    {
                        audioSourceDenis.clip = _gcTextToSpeech.GetAudioClipFromBase64(response.audioContent, Constants.DEFAULT_AUDIO_ENCODING);
                        audioSourceDenis.Play();
                        Debug.Log(audioSourceDenis.clip.length);
                        Invoke("Antworttext_einblenden", audioSourceDenis.clip.length);


                    }

                    else if (Avatar = GameObject.Find("Mike"))
                    {
                        audioSourceMike.clip = _gcTextToSpeech.GetAudioClipFromBase64(response.audioContent, Constants.DEFAULT_AUDIO_ENCODING);
                        audioSourceMike.Play();
                        Debug.Log(audioSourceMike.clip.length);
                        Invoke("Antworttext_einblenden", audioSourceMike.clip.length);
                    }

                    else if (Avatar = GameObject.Find("Remy"))
                    {
                        audioSourceRemy.clip = _gcTextToSpeech.GetAudioClipFromBase64(response.audioContent, Constants.DEFAULT_AUDIO_ENCODING);
                        audioSourceRemy.Play();
                        Debug.Log(audioSourceRemy.clip.length);
                        Invoke("Antworttext_einblenden", audioSourceRemy.clip.length);
                    }

                    else if (Avatar = GameObject.Find("Zoe"))
                    {
                        audioSourceZoe.clip = _gcTextToSpeech.GetAudioClipFromBase64(response.audioContent, Constants.DEFAULT_AUDIO_ENCODING);
                        audioSourceZoe.Play();
                        Debug.Log(audioSourceZoe.clip.length);
                        Invoke("Antworttext_einblenden", audioSourceZoe.clip.length);
                    }

                    else if (Avatar = GameObject.Find("Stefani"))
                    {
                        audioSourceStefani.clip = _gcTextToSpeech.GetAudioClipFromBase64(response.audioContent, Constants.DEFAULT_AUDIO_ENCODING);
                        audioSourceStefani.Play();
                        Debug.Log(audioSourceStefani.clip.length);
                        Invoke("Antworttext_einblenden", audioSourceStefani.clip.length);
                    }

                    else if (Avatar = GameObject.Find("Regina"))
                    {
                        audioSourceRegina.clip = _gcTextToSpeech.GetAudioClipFromBase64(response.audioContent, Constants.DEFAULT_AUDIO_ENCODING);
                        audioSourceRegina.Play();
                        Debug.Log(audioSourceRegina.clip.length);
                        Invoke("Antworttext_einblenden", audioSourceRegina.clip.length);
                    }

                }
          
                //###############################################################################################################

            }

            else if (output is StoryLink)
            {
                var link = (StoryLink)output;
                if (!ShowNamedLinks && link.IsNamed)
                    return;

                //Add Keywords to Dictionary: Ersetzten falls schon vorhanden und sonst insert
                if (actions.ContainsKey(output.Text))
                {
                    actions[output.Text] = link;
                    UpdateKeywordRecognizer();
                }
                else
                {
                    actions.Add(output.Text, link);
                    UpdateKeywordRecognizer();
                }

                //Print Link to console
                Debug.Log("Link: " + output.Text + " / " + link);

                Antworttext(output.Text);

                Button uiLink = (Button)Instantiate(LinkTemplate);
                uiLink.gameObject.SetActive(true);
                uiLink.name = "[[" + link.Text + "]]";

                Text uiLinkText = uiLink.GetComponentInChildren<Text>();
                uiLinkText.text = link.Text;
                uiLink.onClick.AddListener(() =>
                {
                    this.Story.DoLink(link);
                });
                AddToUI((RectTransform)uiLink.transform, output, uiInsertIndex);
            }
            else if (output is LineBreak)
            {
                var br = (RectTransform)Instantiate(LineBreakTemplate);
                br.gameObject.SetActive(true);
                br.gameObject.name = "(br)";
                AddToUI(br, output, uiInsertIndex);
            }
            else if (output is OutputGroup)
            {
                // Add an empty indicator to later positioning
                var groupMarker = new GameObject();
                groupMarker.name = output.ToString();
                AddToUI(groupMarker.AddComponent<RectTransform>(), output, uiInsertIndex);
            }

        }

        void Antworttext(string text)
        {
            antworttext = GameObject.Find("AntwortText").GetComponent<Text>();
            antworttext.text += "\n" + text + "\n";
        }

        void Antworttext_einblenden()//Wird erst angezeigt wenn OutputText gesprochen wurde
        {
            antworttext.GetComponent<Text>().color = Color.white;
        }

        void AddToUI(RectTransform rect, StoryOutput output, int index)
        {
            rect.SetParent(Container);
            if (index >= 0)
                rect.SetSiblingIndex(index);

            var elem = rect.gameObject.AddComponent<TwineTextPlayerElement>();
            elem.SourceOutput = output;
        }

        //Keyword Recognizer
        private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
        {

            if (actions.ContainsKey(speech.text))
            {
                Debug.Log("Keyword erkannt: " + speech.text);
                JumpTo(actions[speech.text]);
            }
        }

        private void JumpTo(StoryLink link)
        {
            Debug.Log("Sprachbefehl erfolgreich");
            this.Story.DoLink(link);

        }

        void UpdateKeywordRecognizer()
        {
            keywordRecognizer.OnPhraseRecognized -= RecognizedSpeech;
            keywordRecognizer.Stop();
            keywordRecognizer.Dispose();
            keywordRecognizer = null;
            keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
            keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
            keywordRecognizer.Start();
        }


    }
}