using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
    public static void LookAt2d(Transform transform, Vector2 targetPos)
    {
        Vector2 diff = targetPos - (Vector2)transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
    }

    public static Vector2 MouseToWorldPos(Camera camera)
    {
        Vector3 mousePos = Input.mousePosition;
        return camera.ScreenToWorldPoint(mousePos);
    }

    public static Vector2 MouseToWorldPos()
    {
        Vector3 mousePos = Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    public static float GetAngle(Vector2 vectorA, Vector2 vectorB)
    {
        Vector2 diff = vectorB - vectorA;
        diff.Normalize();
        float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        return angle;
    }

    public static Vector2 RandomVector2(float min, float max)
    {
        float x = Random.Range(min, max);
        float y = Random.Range(min, max);

        return new Vector2(x, y);
    }

    public static Vector2 RandomVector2(float minX, float maxX, float minY, float maxY)
    {
        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);

        return new Vector2(x, y);
    }

    public static bool GetPlayerPrefsBool(string key, bool defaultValue = false)
    {
        int defaultValueInt = defaultValue ? 1 : 0;
        int value = PlayerPrefs.GetInt(key, defaultValueInt);

        PlayerPrefs.SetInt(key, value);

        if (value == 0)
            return false;
        else
            return true;
    }

    public static void SetPlayerPrefsBool(string key, bool value)
    {
        int intValue = value ? 1 : 0;
        PlayerPrefs.SetInt(key, intValue);
    }
}
