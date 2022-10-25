using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OodModels{
/*
A singleton to give access to centralised game bounds that is managed
*/
public class GameBounds
{
    //Create the private singleton instance
    private static readonly GameBounds _instance = new GameBounds();
    //Private constructor
    private GameBounds(){}
    //Property to encapsulate the _instance
    public static GameBounds Instance {
        get {return _instance;}
        //protected set {_instance = value;}
    }


    private static Vector3 _minBounds;
    //This is the public interface for setting bounds (as 2 Vector3s)
    public static Vector3 minBounds {
        get {return _minBounds;}
        set {
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
    public static Vector3 maxBounds {
        get {return _maxBounds;}
        set {
            _maxBounds = value;
            _maxX = _maxBounds.x;
            _maxY = _maxBounds.y;
            _maxZ = _maxBounds.z;
        }
    }

    //If these are set by subclasses they update the -minBounds and _maxBounds
    private static float _minX;
    public static float minX{
        get {return _minX;}
        protected set {
            if(value == _minX){
                 return;
            }else{
                _minX = value;
                Vector3 temp = new Vector3(_minX,_minY,_minZ);
                _minBounds = temp;
            }
        }
    }
    private static float _maxX;
    public static float maxX{
        get {return _maxX;}
        protected set {
            if(value == _maxX){
                 return;
            }else{
                _maxX = value;
                Vector3 temp = new Vector3(_maxX,_maxY,_maxZ);
                _maxBounds = temp;
            }
        }
    }
    private static float _minY;
    public static float minY{
        get {return _minY;}
        protected set {
            if(value == _minY){
                 return;
            }else{
                _minY = value;
                Vector3 temp = new Vector3(_minX,_minY,_minZ);
                _minBounds = temp;
            }
        }
    }
    private static float _maxY;
    public static float maxY{
        get {return _maxY;}
        protected set {
            if(value == _maxY){
                 return;
            }else{
                _maxY = value;
                Vector3 temp = new Vector3(_maxX,_maxY,_maxZ);
                _maxBounds = temp;
            }
        }
    }
    private static float _minZ;
    public static float minZ{
        get {return _minZ;}
        protected set {
            if(value == _minZ){
                 return;
            }else{
                _minZ = value;
                Vector3 temp = new Vector3(_minX,_minY,_minZ);
                _minBounds = temp;
            }
        }
    }
    private static float _maxZ;
    public static float maxZ{
        get {return _maxZ;}
        protected set {
            if(value == _maxZ){
                 return;
            }else{
                _maxZ = value;
                Vector3 temp = new Vector3(_maxX,_maxY,_maxZ);
                _maxBounds = temp;
            }
        }
    }
}
}
