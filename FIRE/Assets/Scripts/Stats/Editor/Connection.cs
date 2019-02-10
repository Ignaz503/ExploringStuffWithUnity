using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Connection
{
    Func<Rect> inPoint, outPoint;

    public Rect InPoint
    {
        get
        {
            return inPoint();
        }
    }

    public Rect OutPoint
    {
        get
        {
            return outPoint();
        }
    }

    public Action<Connection> OnClickRemoveConnection;
    NodeBasedEditor editorBelongingTo;

    protected float tangentMulitplier { get { return editorBelongingTo.Settings.ConnectionBezierTangentMultiplier; } }

    protected float bezierWidth { get { return editorBelongingTo.Settings.ConnectionBezierWidth; } }

    protected Color connectionColor { get { return editorBelongingTo.Settings.ConnectionColor; } }

    protected Texture2D connectionTexture { get { return editorBelongingTo.Settings.ConnectionBezierTexture; } }

    protected float buttonSize { get { return editorBelongingTo.Settings.ConnectionButtonSize; } }

    protected Vector2 inPointBezierStart
    {
        get
        {
            //return inPoint.rect.center;
            return GetClosestPointRect(InPoint, OutPoint.center);
        }
    }

    protected Vector2 outPointBezierStart
    {
        get
        {
            //return outPoint.rect.center;
            return GetClosestPointRect(OutPoint, InPoint.center);
        }
    }

    protected Vector2 inPointBezierTangent
    {
        get
        {
            Vector2 inPoBez = inPointBezierStart;
            Vector2 tangentMod = Vector2.left;

            Vector2 dir = inPoBez - InPoint.center;

            if (outPointBezierStart.x > inPoBez.x)
                tangentMod = Vector2.right;

            return inPoBez + tangentMod * tangentMulitplier;
        }
    }

    protected Vector2 outPointBezierTangent
    {
        get
        {
            Vector2 outPoBez = outPointBezierStart;
            Vector2 tangentMod = Vector2.left;

            if (outPoBez.x > inPointBezierStart.x)
                tangentMod = Vector2.right;

            return outPointBezierStart - tangentMod * tangentMulitplier;
        }
    }

    protected Vector2 GetClosestPointRect(Rect r, Vector2 pos)
    {
        Vector2 upperLeft = new Vector2(r.xMin, r.yMin);
        Vector2 upperRight = new Vector2(r.xMax, r.yMin);
        Vector2 lowerRight = new Vector2(r.xMax, r.yMax);
        Vector2 lowerLeft = new Vector2(r.xMin, r.yMax);

        Vector2 uRuLPoint = Vector2.Lerp(upperRight, upperLeft, .5f);
        Vector2 uRlRPoint = Vector2.Lerp(upperRight, lowerRight, .5f);
        Vector2 uLlLPoint = Vector2.Lerp(upperLeft, lowerLeft, .5f);
        Vector2 lRlLPoint = Vector2.Lerp(lowerRight, lowerLeft, .5f);

        Vector2 closest = r.center;
        float minSqr = float.MaxValue;
        
        if((pos-uRuLPoint).sqrMagnitude < minSqr)
        {
            minSqr = (pos - uRuLPoint).sqrMagnitude;
            closest = uRuLPoint;
        }

        if ((pos - uRlRPoint).sqrMagnitude < minSqr)
        {
            minSqr = (pos - uRlRPoint).sqrMagnitude;
            closest = uRlRPoint;
        }
        if ((pos - uLlLPoint).sqrMagnitude < minSqr)
        {
            minSqr = (pos - uLlLPoint).sqrMagnitude;
            closest = uLlLPoint;
        }
        if ((pos - lRlLPoint).sqrMagnitude < minSqr)
        {
            minSqr = (pos - lRlLPoint).sqrMagnitude;
            closest = lRlLPoint;
        }
        return closest;
    }

    public virtual GenericMenu ConnectionMenu
    {
        get
        {
            GenericMenu menu = new GenericMenu();
            AddRemoveItemToGenericMenu(menu);
            return menu;
        }
    }

    public Connection(Func<Rect> inPoint, Func<Rect> outPoint, Action<Connection> onClickRemoveConnection, NodeBasedEditor editorBelongingTo)
    {
        if (inPoint == null || outPoint == null)
            throw new Exception("Can't have null functions for in and out point");

        this.inPoint = inPoint;
        this.outPoint = outPoint;
        this.editorBelongingTo = editorBelongingTo;
        OnClickRemoveConnection = onClickRemoveConnection;
    }

    public virtual void Draw()
    {
        Handles.DrawBezier(inPointBezierStart, outPointBezierStart, inPointBezierTangent, outPointBezierTangent, connectionColor, connectionTexture, bezierWidth);
        DrawDisc();
        DrawHandlesButton();
    }

    protected void DrawHandlesButton()
    {
        if (Handles.Button((inPointBezierStart + outPointBezierStart) * .5f, Quaternion.identity, buttonSize, buttonSize * 2f, Handles.RectangleHandleCap))
        {
            ConnectionMenu.ShowAsContext();
        }
    }

    protected void DrawDisc()
    {
        Handles.DrawSolidDisc(outPointBezierStart, Vector3.forward, 5f);
    }

    protected void AddRemoveItemToGenericMenu(GenericMenu menu)
    {
        menu.AddItem(new GUIContent("Remove Connection"), false, () => OnClickRemoveConnection?.Invoke(this));
    }
}

public class StatInfluenceConnection : Connection
{

    public float Influence { get; protected set; }
    public int Influencer { get; protected set; }
    public int Influenced { get; protected set; }

    public StatInfluenceConnection(Func<Rect> inPoint, Func<Rect> outPoint, Action<Connection> onClickRemoveConnection, NodeBasedEditor editorBelongingTo,int influencer, int influenced, float influence = 0) : base(inPoint, outPoint, onClickRemoveConnection, editorBelongingTo)
    {
        Influencer = influencer;
        Influenced = influenced;
        Influence = influence;
    }

    public override GenericMenu ConnectionMenu
    {
        get
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Influence"), false, ShowInfluenceEditor);

            AddRemoveItemToGenericMenu(menu);
            return menu;
        }
    }

    void ShowInfluenceEditor()
    {
        Vector3 pos = (inPointBezierStart + outPointBezierStart) * .5f;

        InfluenceEditWindow newWind = ScriptableObject.CreateInstance<InfluenceEditWindow>();
        newWind.position = new Rect(pos, Vector2.up * 25f + Vector2.right * 150f);
        newWind.Initialize(this);
        newWind.Show();
    }

    public override void Draw()
    {
        Handles.DrawBezier(inPointBezierStart, outPointBezierStart, inPointBezierTangent, outPointBezierTangent, connectionColor, connectionTexture, bezierWidth*(1f+Influence));
        DrawDisc();
        DrawHandlesButton();
    }

    public class InfluenceEditWindow : EditorWindow
    {
        StatInfluenceConnection influence;
        public void Initialize(StatInfluenceConnection influenceConn)
        {
            influence = influenceConn;
        }

        private void OnGUI()
        {
            string value = EditorGUILayout.DelayedTextField("Influence: ", influence.Influence.ToString(), new GUIStyle());
            float res = 0f;
            if(float.TryParse(value,out res))
            {
                influence.Influence = res;
            }
        }
    }

}

