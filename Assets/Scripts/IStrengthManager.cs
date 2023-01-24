namespace IModels
{
    public interface IStrengthManager
    {
        public float strength {
            get; //protected set;
        }
        public float startStrength {
            get; //protected set;
        }
        //set by artist and used for strength slider UI
        public float maxStrength {
            get; //protected set;
        }
        //called before use
        public void Enable();
        //called to pause/hide
        public void Disable();
        //reset to start strength
        public void Reset();
        //set up how your going to present your strength
        public abstract void EnableUI();
        //hide the UI
        public abstract void DisableUI();
        //called whenever strength is changed to keep the UI representation up to date
        public abstract void UpdateUI();
        //use to remove from strength, returns a bool, false if run out of strength
        public bool AddDamageLevel(float toRemove);
        //the opposite of AddDamageLevel
        public void AddStrengthLevel(float toAdd);

    }
}