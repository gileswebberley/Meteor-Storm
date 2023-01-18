using UnityEngine;
using TMPro;

namespace OodModels
{
    public class ScoreManagerUI : ScoreManager
    {
        //ScoreManager _scorer = new ScoreManager();
        [SerializeField] TextMeshProUGUI textUI;
        //this doesn't work as expected, perhaps because I am making it with the new keyword??
        public bool CreateScoreTextArea(string textAreaName)
        {
            textUI = GameObject.Find(textAreaName).GetComponent<TextMeshProUGUI>();
            if(textUI == null){
                Debug.Log("Cannot find the score text area named: "+textAreaName);
                return false;
            }
            else return true;
        }

        public void SetScoreTextArea(TextMeshProUGUI textArea)
        {
            textUI = textArea;
        }

        public void Show()
        {
            textUI.gameObject.SetActive(true);
        }

        public void Hide()
        {
            textUI.gameObject.SetActive(false);
        }

        public override void AddToScore(int toAdd)
        {
            base.AddToScore(toAdd);
            UpdateScoreUI();
        }

        public override void RemoveFromScore(int toRemove)
        {
            base.RemoveFromScore(toRemove);
            UpdateScoreUI();
        }

        public override void ResetScore()
        {
            base.ResetScore();
            UpdateScoreUI();
        }

        void UpdateScoreUI()
        {
            textUI.text = $"{_thisScore.name} score:{_thisScore.score}";
        }
    }
}
