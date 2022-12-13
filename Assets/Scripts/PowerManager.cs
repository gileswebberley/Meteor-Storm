using UnityEngine;
using IModels;

namespace GilesManagers
{
    public class PowerManager : StrengthManagerBase, IStrengthManager
    {
        //The game object which contains the sliding indicator
        [SerializeField, TooltipAttribute("Must have a moveable gameobject called 'Indicator' as a child object")]
        private GameObject powerIndicator;
        //the sliding gameobject itself which is a child of powerIndicator called "Indicator"
        private GameObject indicatorFill;
        [SerializeField, TooltipAttribute("set this to approx. double the Power Indicator offset position")]
        private float indicatorMoveRange;
        //amount to move the sliding component of the UI above
        private float indicatorMoveStep;

        public override void EnableUI()
        {
            //use transform.Find for children game objects
            indicatorFill = powerIndicator.transform.Find("Indicator").gameObject;
            //the float is the distance to move the power cylinder forwards to create the nice effect
            indicatorMoveStep = indicatorMoveRange / _maxStrength;
        }

        public override void UpdateUI()
        {
            //Frustratingly it took me ages to realise that I had to use localPosition
            indicatorFill.transform.localPosition = new Vector3(0.0f, indicatorMoveStep * _strength, 0.0f);
        }
    }
}
