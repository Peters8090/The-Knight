using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ally : Knight
{
    #region Component References

    protected Light highlight;

    #endregion

    /// <summary>
    /// Returns all instances of Ally class with allyRank equal either Member or Boss
    /// </summary>
    public static Ally[] Allies
    {
        get
        {
            return FindObjectsOfType<Ally>();
        }
    }

    protected override void Start()
    {
        base.Start();

        highlight = transform.Find("Point Light").GetComponent<Light>();
    }

    protected override void LateUpdate()
    {
        if (GameControlScript.gameOver)
            return;

        base.LateUpdate();

        transform.localEulerAngles = Vector3.zero;
    }

    /// <summary>
    /// Change ally rank to Rank (add proper class to the gameObject and remove previous)
    /// </summary>
    /// <typeparam name="Rank"></typeparam>
    internal protected void ChangeAllyRankTo<Rank>() where Rank : Component
    {
        if (gameObject.GetComponent<Rank>())
            return;
        gameObject.AddComponent<Rank>();
        Destroy(this);
    }
}
