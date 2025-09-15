using System;
using Controller;
using UnityEngine;

public static class EventManager
{
    public static Action attackInput;
    public static Action<PlayerController> UpdateStunAction;
    public static Action UpdateFlagP1;
    public static Action UpdateFlagP2;
}
