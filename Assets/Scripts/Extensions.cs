using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public static class Extensions
{
    public static void AddScalingToButton(this Button button)
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry1 = new EventTrigger.Entry();
        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.PointerEnter;
        entry2.eventID = EventTriggerType.PointerExit;
        Tween tween = null;
        entry1.callback.AddListener(delegate { if (tween != null) DOTween.Kill(tween); tween = button.transform.DOScale(1.2f, 0.2f); });
        entry2.callback.AddListener(delegate { if (tween != null) DOTween.Kill(tween); tween = button.transform.DOScale(1.0f, 0.2f); });
        trigger.triggers.Add(entry1);
        trigger.triggers.Add(entry2);
    }
    public static void DestroyAllFirstLevelChildren(this Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            GameObject.Destroy(parent.GetChild(i).gameObject);
        }
    }
    public static GameObject[] GetFirstLevelChildren(this Transform parent)
    {
        List<GameObject> children = new List<GameObject>();
        for (int i = 0; i < parent.childCount; i++)
        {
            children.Add(parent.GetChild(i).gameObject);
        }
        return children.ToArray();
    }
    public static Type[] GetInheritedClasses<T>()
    {
        return Assembly.GetAssembly(typeof(T)).GetTypes().Where(TheType => TheType.IsClass && !TheType.IsAbstract && TheType.IsSubclassOf(typeof(T))).ToArray();
    }
    public static T[] GetObjectsInAreaWithComponent<T>(Vector2 center, float radius)
    {
        IEnumerable<Collider2D> colliders = Physics2D.OverlapCircleAll(center, radius).Where(collider => collider.GetComponentInChildren<T>() != null);
        T[] componentlist = new T[colliders.Count()];
        for (int i = 0; i < componentlist.Length; i++)
        {
            componentlist[i] = colliders.ElementAt(i).GetComponentInChildren<T>();
        }
        return componentlist.ToArray();
    }
    public static T[] GetObjectsInAreaWithComponent<T>(Vector2 center, float radius, LayerMask layerMask)
    {
        IEnumerable<Collider2D> colliders = Physics2D.OverlapCircleAll(center, radius, layerMask).Where(collider => collider.GetComponentInChildren<T>() != null);
        T[] componentlist = new T[colliders.Count()];
        for (int i = 0; i < componentlist.Length; i++)
        {
            componentlist[i] = colliders.ElementAt(i).GetComponentInChildren<T>();
        }
        return componentlist.ToArray();
    }
    public static T[] GetObjectsInAreaWithComponent<T>(Vector2 center, Vector2 roomSize, float angle)
    {
        IEnumerable<Collider2D> colliders = Physics2D.OverlapBoxAll(center, roomSize, angle).Where(collider => collider.GetComponentInChildren<T>() != null).ToArray();
        T[] componentlist = new T[colliders.Count()];
        for (int i = 0; i < componentlist.Length; i++)
        {
            componentlist[i] = colliders.ElementAt(i).GetComponentInChildren<T>();
        }
        return componentlist.ToArray();
    }
    public static T[] GetObjectsInAreaWithComponent<T>(Vector2 center, Vector2 roomSize, float angle, LayerMask layerMask)
    {
        IEnumerable<Collider2D> colliders = Physics2D.OverlapBoxAll(center, roomSize, angle, layerMask).Where(collider => collider.GetComponentInChildren<T>() != null).ToArray();
        T[] componentlist = new T[colliders.Count()];
        for (int i = 0; i < componentlist.Length; i++)
        {
            componentlist[i] = colliders.ElementAt(i).GetComponentInChildren<T>();
        }
        return componentlist.ToArray();
    }
}
