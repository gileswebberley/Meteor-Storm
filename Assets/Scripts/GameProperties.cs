using System.Collections.Generic;

//custom namespace created for my attempts at refactoring Meteor Storm
//haven't implemented this so far, and have since discovered that Unity
//has a game properties system, although it's good to work through problems
//to learn - so new to C# that I hadn't worked with Dictionaries before
namespace GilesManagers
{
    //I'm thinking of making a central point for objects in the game to find
    //out basic info about others without having to access the GameObject Component itself
    //basically a wrapper for a Dictionary, I think it would be good to catch exceptions etc
    public class GameProperties
    {
        //Create the private singleton instance
        private static readonly GameProperties _instance = new GameProperties();
        //Property to encapsulate the _instance
        //needed to access non-static members
        public static GameProperties Instance
        {
            get { return _instance; }
        }
        //Private constructor
        private GameProperties()
        {
            if(Properties != null) return;
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

        private static Dictionary<string, object> Properties;

        //Going to attempt what I think is called generics??
        public T GetGameProperty<T>(string key)
        {
            if(Properties.ContainsKey(key)){
                return (T)Properties[key];
            }
            return default(T);
        }

        public void AddGameProperty<T>(string key, T val)
        {
            SetGameProperty(key, val);
        }

        //Avoid exceptions by checking for duplicated keys
        public bool SetGameProperty<T>(string key, T val)
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
