using System;
using TouchlessA3D;
using UnityEngine;

public class ITouchlessSession : MonoBehaviour {
  [HideInInspector]
  public static ITouchlessSession instance;
  [HideInInspector]
  public EventHandler<GestureEvent> ta3dEventHandler;
}