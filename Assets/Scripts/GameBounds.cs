using System.Collections.Generic;
using System.Collections;
using UnityEngine;

//custom namespace created for my attempts at refactoring Meteor Storm
namespace OodModels
{
    /*
    A singleton to give access to centralised game bounds that is managed
    */
    public class GameBounds
    {
        //Create the private singleton instance
        private static readonly GameBounds _instance = new GameBounds();
        //Private constructor
        private GameBounds() { }
        //Property to encapsulate the _instance
        //only needed to access non-static members
        public static GameBounds Instance
        {
            get { return _instance; }
        }

        /*
        To do the same but with a MonoBehaviour subclass
        private static GameBounds _instance;
        public static GameBounds Instance{
            get {return _instance;}
            protected set {_instance = value;}
        }

        private void Awake()
        {
            //make sure there's only one otherwise it's not much of a singleton
            if (Instance != null)
            {
                //this is not the first instance of MainManager so our singleton already exists
                Destroy(gameObject);
                //no need to carry on with script...
                return;
            }
            Instance = this;
            //to make persistant between scenes
            DontDestroyOnLoad(gameObject);
        }
        */


        private static Vector3 _minBounds;
        //This is the public interface for setting bounds (as 2 Vector3s)
        public static Vector3 minBounds
        {
            get { return _minBounds; }
            set
            {
                _minBounds = value;
                //keep the seperate values updated
                //by directly setting the private fields rather than through
                //their Property I hope it will mean that it doesn't 'run'
                //their getters/setters? Important or else we'll be making 
                //infinite nubers of Vector3s!!?
                _minX = _minBounds.x;
                _minY = _minBounds.y;
                _minZ = _minBounds.z;
            }
        }
        private static Vector3 _maxBounds;
        public static Vector3 maxBounds
        {
            get { return _maxBounds; }
            set
            {
                _maxBounds = value;
                _maxX = _maxBounds.x;
                _maxY = _maxBounds.y;
                _maxZ = _maxBounds.z;
            }
        }

        //If these are set by subclasses they update the -minBounds and _maxBounds
        private static float _minX;
        public static float minX
        {
            get { return _minX; }
            protected set
            {
                if (value == _minX)
                {
                    return;
                }
                else
                {
                    _minX = value;
                    Vector3 temp = new Vector3(_minX, _minY, _minZ);
                    _minBounds = temp;
                }
            }
        }
        private static float _maxX;
        public static float maxX
        {
            get { return _maxX; }
            protected set
            {
                if (value == _maxX)
                {
                    return;
                }
                else
                {
                    _maxX = value;
                    Vector3 temp = new Vector3(_maxX, _maxY, _maxZ);
                    _maxBounds = temp;
                }
            }
        }
        private static float _minY;
        public static float minY
        {
            get { return _minY; }
            protected set
            {
                if (value == _minY)
                {
                    return;
                }
                else
                {
                    _minY = value;
                    Vector3 temp = new Vector3(_minX, _minY, _minZ);
                    _minBounds = temp;
                }
            }
        }
        private static float _maxY;
        public static float maxY
        {
            get { return _maxY; }
            protected set
            {
                if (value == _maxY)
                {
                    return;
                }
                else
                {
                    _maxY = value;
                    Vector3 temp = new Vector3(_maxX, _maxY, _maxZ);
                    _maxBounds = temp;
                }
            }
        }
        private static float _minZ;
        public static float minZ
        {
            get { return _minZ; }
            protected set
            {
                if (value == _minZ)
                {
                    return;
                }
                else
                {
                    _minZ = value;
                    Vector3 temp = new Vector3(_minX, _minY, _minZ);
                    _minBounds = temp;
                }
            }
        }
        private static float _maxZ;
        public static float maxZ
        {
            get { return _maxZ; }
            protected set
            {
                if (value == _maxZ)
                {
                    return;
                }
                else
                {
                    _maxZ = value;
                    Vector3 temp = new Vector3(_maxX, _maxY, _maxZ);
                    _maxBounds = temp;
                }
            }
        }

        //Trying to use ref for the first time, I tried using out and was warned about properties may not exist
        //moving the functionality in here where it belongs, rather than part of the PlayerController
        public bool CheckForXYBounds(Vector3 comparePosition, ref Vector3 targetVector)
    {
        // Vector3 temp = new Vector3(1, 1, 1);
        // Vector3 tempMoveMe = new Vector3(0, 0, 0);
        bool isHittingBounds = false;
        //gonna try to move this to the GameBounds class as a method
        //Vector3 tempTransform = new Vector3(transform.position.x + targetVector.x, transform.position.y + targetVector.y, 1);
        //gone too wide so push us back into the middle and return [0,y,1] 
        if (comparePosition.x + targetVector.x > GameBounds.maxX)
        {
            // temp.x = 0;
            targetVector.x = -1;
            isHittingBounds = true;
        }
        else if (comparePosition.x + targetVector.x < GameBounds.minX)
        {
            // temp.x = 0;
            targetVector.x = 1;
            isHittingBounds = true;
        }
        //gone too high or low so push us back into the middle and return[x,0,1]
        if (comparePosition.y + targetVector.y > GameBounds.maxY)
        {
            // temp.y = 0;
            targetVector.y = -1;
            isHittingBounds = true;
        }
        else if (comparePosition.y + targetVector.y < GameBounds.minY)
        {
            //temp.y = 0;
            targetVector.y = 1;
            isHittingBounds = true;
        }
        //if (isHittingBounds) MoveMe(tempMoveMe);
        return isHittingBounds;
    }
    }
}
