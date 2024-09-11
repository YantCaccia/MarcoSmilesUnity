
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
///<summary>
/// Incapsula i dati relativi alla configurazione di una mano, ogni parametro rappresenta la distanza di quel joint dal joint del palmo.
///</summary>
public class DataToStore
{

    
    public string Hand;

    public float TMd;
    public float TPd;
    public float TDd;
    public float TTd;
    public float IMd;
    public float IPd;
    public float IId;
    public float IDd;
    public float ITd;
    public float MMd ;  
    public float MPd ;  
    public float MId ;  
    public float MDd ;   
    public float MTd ;  
    public float RMd ;  
    public float RPd ;
    public float RId ;
    public float RDd ;
    public float RTd ;
    public float LMd ; 
    public float LPd ;
    public float LId ;
    public float LDd ;
    public float LTd ;
 


    public DataToStore() { }


    public DataToStore(string hand, float tMd, float tPd, float tDd, float tTd, 
                        float iMd, float iPd, float iId, float iDd, float iTd,
                        float mMd, float mPd, float mId, float mDd, float mTd, float rMd, float rPd, float rId, float rDd, float rTd, float lMd, float lPd, float lId, float lDd, float lTd)
    {
        this.Hand = hand;
        this.TMd = tMd;
        this.TPd = tPd;
        this.TDd = tDd;
        this.TTd = tTd;
        this.IMd = iMd;
        this.IPd = iPd;
        this.IId = iId;
        this.IDd = iDd;
        this.ITd = iTd;
        this.MMd = mMd;
        this.MPd = mPd;
        this.MId = mId;
        this.MDd = mDd;
        this.MTd = mTd;
        this.RMd = rMd;
        this.RPd = rPd;
        this.RId = rId;
        this.RDd = rDd;
        this.RTd = rTd;
        this.LMd = lMd;
        this.LPd = lPd;
        this.LId = lId;
        this.LDd = lDd;
        this.LTd = lTd;
    }

    public override string ToString()
    {
        return base.ToString() +
           $"Hand = {Hand}, TMd = {TMd}, TPd = {TPd}, TDd = {TDd}, TTd = {TTd}, " +
           $"IMd = {IMd}, IPd = {IPd}, IId = {IId}, IDd = {IDd}, ITd = {ITd}, " +
           $"MMd = {MMd}, MPd = {MPd}, MId = {MId}, MDd = {MDd}, MTd = {MTd}, " +
           $"RMd = {RMd}, RPd = {RPd}, RId = {RId}, RDd = {RDd}, RTd = {RTd}, " +
           $"LMd = {LMd}, LPd = {LPd}, LId = {LId}, LDd = {LDd}, LTd = {LTd}";
    }

    public List<float> FeatureLists()
    {
        
        List<float> Features = new List<float>();
        Features.Add(TMd);
        Features.Add(TPd);
        Features.Add(TDd);
        Features.Add(TTd);
        Features.Add(IMd);
        Features.Add(IPd);
        Features.Add(IId);
        Features.Add(IDd);
        Features.Add(ITd);
        Features.Add(MMd);
        Features.Add(MPd);
        Features.Add(MId);
        Features.Add(MDd);
        Features.Add(MTd);
        Features.Add(RMd);
        Features.Add(RPd);
        Features.Add(RId);
        Features.Add(RDd);
        Features.Add(RTd);
        Features.Add(LMd);
        Features.Add(LPd);
        Features.Add(LId);
        Features.Add(LDd);
        Features.Add(LTd);

        return Features;

    }
}
