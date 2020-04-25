﻿using UnityEngine;
using UnityEngine.UI;

namespace Assets.EVE.Scripts.Menu.Buttons
{
    public class ErrorMenuButtons : MonoBehaviour{

        private string _originBaseMenu;
        private string _originContext;
        private LaunchManager _launchManager;

        void Start()
        {
            _launchManager = GameObject.FindGameObjectWithTag("LaunchManager").GetComponent<LaunchManager>();
            var btn = transform.Find("Panel").Find("Fields").Find("OkButton").GetComponent<Button>();
            btn.onClick.AddListener(ShowOriginMenu);
        }

        /// <summary>
        /// String to be displayed as error message.
        /// </summary>
        /// <param name="errorText">Error Message</param>
        public void SetErrorText(string errorText)
        {
            var errorMessageObj=GameObject.Find("ErrorMessage");
            errorMessageObj.GetComponent<Text>().text = errorText;
        }

        /// <summary>
        /// Menu which to display when closing the error message.
        /// </summary>
        /// <param name="originBaseMenu">Menu to be displayed.</param>
        public void SetErrorOriginMenu(string originBaseMenu, string originContext)
        {
            _originBaseMenu = originBaseMenu;
            _originContext = originContext;
        }

        /// <summary>
        /// Returns to previous menu.
        /// </summary>
        public void ShowOriginMenu() {
            if (_originBaseMenu == "Questionnaire" && _originContext == "QuestionnaireSystem")
            {
                _launchManager.QuestionnaireManager.ContinueFromError();
            }
            else
            {
                _launchManager.MenuManager.InstantiateAndShowMenu(_originBaseMenu, _originContext);
            }
        }
    }
}
