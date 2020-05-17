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
        public bool StartStory = true;
        public bool AutoDisplay = true;
        public bool ShowNamedLinks = true;

        static Regex rx_splitText = new Regex(@"(\s+|[^\s]+)");


        private GCTextToSpeech _gcTextToSpeech; //Google Text to Speech


        public AudioSource audioSource; //Audioquelle


        public Text antworttext;
        GameObject AntwortDisplay;

        public String RemyVoice;
        public String MikeVoice;
        public String DenisVoice;
        public String ZoeVoice;
        public String StefaniVoice;
        public String ReginaVoice;
        public String Language;

        GameObject Avatar;

        private String Entiretext;//Text in Variable abspeichern, damit auf Leerzeilen verwendet werden können
        private float Lenght;

        //KeywordRecognizer
        private KeywordRecognizer keywordRecognizer;
        private Dictionary<string, StoryLink> actions = new Dictionary<string, StoryLink>();

        //Warte 5 Sekunden bevor Story beginnt
        private float startWait = 5f;


        void Start()
        {
            Invoke("Initialize", startWait);
            Debug.Log("Warte " + startWait + " Sekunden");
            audioSource = GameObject.FindGameObjectWithTag("Audiosource").GetComponent<AudioSource>();// Zuweisen von Audioquelle
        }


        void Initialize()
        {

            if (!Application.isPlaying)
                return;

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

                Entiretext += text.Text; //Einzelne Zeilen in Variable abspeichern

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
                    languageCode = "" + Language + ""
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
                        name = "" + RemyVoice + ""
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
                    audioSource.clip = _gcTextToSpeech.GetAudioClipFromBase64(response.audioContent, Constants.DEFAULT_AUDIO_ENCODING);
                    audioSource.Play();
                    Invoke("Verzoegerung", 0.5f);
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

        void Verzoegerung()//Zwischenschritt, dass Verzögerung für Linebreak funktioniert
        {
            Lenght = Mathf.Max(audioSource.clip.length);
            Debug.Log(Lenght);
            Invoke("Antworttext_einblenden", Lenght);
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