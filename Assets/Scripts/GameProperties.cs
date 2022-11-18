using System.Collections.Generic;

//custom namespace created for my attempts at refactoring Meteor Storm
namespace OodModels
{
    //I'm thinking of making a central point for objects in the game to find
    //out basic info about others without having to access the GameObject Component itself
    //basically a wrapper for a Dictionary, I think it would be good to catch exceptions etc
    public class GameProperties
    {
        //Create the private singleton instance
        private static readonly GameProperties _instance = new GameProperties();
        //Private constructor
        private GameProperties()
        {
            Properties = new Dictionary<string, object>()
            {
                //set up some default properties
                {"Player Speed",null},
                {"Player Strength", null},
                {"PLayer Damage", null},
                {"Weapon Power",null},
                {"Difficulty",null},

            };
        }
        //Property to encapsulate the _instance
        //only needed to access non-static members
        public static GameProperties Instance
        {
            get { return _instance; }
        }

        private Dictionary<string, object> Properties;

        public object GetGameProperty(string key)
        {
            if(Properties.ContainsKey(key)){
                return Properties[key];
            }
            return null;
        }

        public void AddGameProperty(string key, object val)
        {
            SetGameProperty(key, val);
        }

        //Avoid exceptions by checking for duplicated keys
        public bool SetGameProperty(string key, object val)
        {
            //check so it doesn't throw an exception and presume this is the value it should hold
            //perhaps should put some kind of back-up
            if (Properties.ContainsKey(key))
            {
                Properties[key] = val;
                //It already existed and has been set
                return true;
            }
            else
            {
                Properties.Add(key, val);
                //it didn't exist but has been added
                return false;
            }
        }
    }
}
