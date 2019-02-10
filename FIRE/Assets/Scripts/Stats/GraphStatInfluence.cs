using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GraphStatInfluence
{
    public class StatConnection
    {
        public int Influencer { get; protected set; }
        Dictionary<int, Influenced> influenced;
        public IEnumerable<Influenced> InfluencedStats { get { return influenced.Values; } }

        public StatConnection(int influencer)
        {
            Influencer = influencer;
            influenced = new Dictionary<int, Influenced>();
        }

        public void AddInfluenced(int infled, float val, bool overrideValueIfAlreadyInfluenced = true)
        {
            Influenced inf = new Influenced(infled, val);
            if (influenced.ContainsKey(infled))
            {
                if (overrideValueIfAlreadyInfluenced)
                    influenced[infled] = inf;
            }
            else
            {
                influenced.Add(infled, inf);
            }

        }

        public struct Influenced
        {
            public int idx;
            public float val;

            public Influenced(int idx, float val)
            {
                this.idx = idx;
                this.val = val;
            }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return idx;
            }

            public override string ToString()
            {
                return base.ToString();
            }
        }
    }

    Dictionary<int, StatConnection> statConnections;
    public IEnumerable<StatConnection> StatConnections { get { return statConnections.Values; } }

    public int Count { get { return statConnections.Values.Count; } }

    public GraphStatInfluence()
    {
        statConnections = new Dictionary<int, StatConnection>();
    }

    public void AddStatConnection(int influencer, int influenced, float influence, bool overrideValueIfAlreadyInfluenced = true)
    {
        if (!statConnections.ContainsKey(influencer))
        {
            statConnections.Add(influencer, new StatConnection(influencer));
            statConnections[influencer].AddInfluenced(influenced, influence, overrideValueIfAlreadyInfluenced);
        }
        else
        {
            statConnections[influencer].AddInfluenced(influenced, influence, overrideValueIfAlreadyInfluenced);
        }
    }

    public void ChangeInfluence(int influencer, int influenced, float newVal, bool createIfNonExistent)
    {
        if (statConnections.ContainsKey(influencer))
        {
            statConnections[influencer].AddInfluenced(influenced, newVal);
        }
        else
        {
            AddStatConnection(influencer, influenced, newVal);
        }
    }

    public void Replace(int influencer, StatConnection conection)
    {
        if (statConnections.ContainsKey(influencer))
            statConnections[influencer] = conection;
    }

    public void Remove(StatConnection con)
    {
        statConnections.Remove(con.Influencer);
    }

    public void Remove(int influencer)
    {
        statConnections.Remove(influencer);
    }

    public static GraphStatInfluence CreateStatInfluenceGraph(StatInfluenceGraphStorage storage)
    {
        GraphStatInfluence infGraph = new GraphStatInfluence();

        if (storage.ConnectionInfos == null)
            Debug.Log("uhh what late");

        foreach(StatInfluenceConnectionInfo info in storage.ConnectionInfos)
        {
            infGraph.AddStatConnection(info.Influencer, info.Influenced, info.Value);
        }

        return infGraph;
    }

}
