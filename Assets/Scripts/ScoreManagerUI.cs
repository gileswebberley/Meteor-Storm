using UnityEngine;
using TMPro;

namespace OodModels
{
    public class ScoreManagerUI : ScoreManager
    {
        [SerializeField] TextMeshProUGUI textUI;
        //this doesn't work as expected, perhaps because I am making it with the new keyword
        //changed ScoreManager to a scriptable object and create an instance in ScoringSystem
        public bool CreateScoreTextArea(string textAreaName)
        {
            try{
            textUI = GameObject.Find(textAreaName).GetComponent<TextMeshProUGUI>();
            Debug.Log("Found score text area");
            return true;
            }
            catch{
                Debug.LogError("Cannot find the score text area named: "+textAreaName);
                return false;
            }
        }

        public void SetScoreTextArea(TextMeshProUGUI textArea)
        {
            if(textUI != null) textUI = textArea;
        }

        public void Show()
        {
            if(textUI != null) textUI.gameObject.SetActive(true);
            UpdateScoreUI();
        }

        public void Hide()
        {
            if(textUI != null) textUI.gameObject.SetActive(false);
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
            if(textUI != null) textUI.text = $"{_thisScore.name} score:{_thisScore.score}";
        }
    }
}
