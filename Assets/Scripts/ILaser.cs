namespace IModels
{
    public interface ILaser
    {
        //Weaponisation related items
        //how much power each round has so the victims can check how much to remove from themselves
        float laserPower { get;}
        //number of rounds per full power load (so number of shots with max power)
        float laserPowerUsageDivisor { get;}
        //originally had this as maxPower so it would be 1 when on full power - deprecated
        //private float laserPowerDivisor;
        //lock used for Firing input control
        bool bIsFiring { get;}
        //time step control for Firing input
        float roundsPerSecond { get;}

        abstract bool Fire();
    }
}