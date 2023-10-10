using System;
using System.Collections.Generic;
using UnityEngine;

enum ArrayType
{
    transType,
    vectorType
}

public class To
{
    private Transform[] vet2Points;
    private Transform[] vet3Points;
    private Vector2[] vet2s;
    private Vector3[] vet3s;

    ArrayType _arrayType;


    /// <summary>
    /// 2D直角坐标转极坐标
    /// </summary>
    /// <param name="vt2">2D直角坐标向量</param>
    /// <returns>极坐标向量</returns>
    Vector2 CartesianToPolar(Vector2 vt2)
    {
        float r = Mathf.Sqrt(vt2.x * vt2.x + vt2.y * vt2.y);
        float deg = Mathf.Atan2(vt2.y, vt2.x) * Mathf.Rad2Deg;
        return new Vector2(r, deg);
    }
    /// <summary>
    /// 3D直角坐标转极坐标
    /// </summary>
    /// <param name="vt3">3D直角坐标向量</param>
    /// <returns>3D极坐标向量</returns>
    Vector3 CartesianToPolar(Vector3 vt3)
    {
        float r = Mathf.Sqrt(Mathf.Pow(vt3.x, 2) + Mathf.Pow(vt3.y, 2) + Mathf.Pow(vt3.z, 2));
        float deg = Mathf.Acos(vt3.z / Mathf.Sqrt(Mathf.Pow(vt3.x, 2) + Mathf.Pow(vt3.y, 2) + Mathf.Pow(vt3.z, 2))) * Mathf.Rad2Deg;
        float dirDeg = Mathf.Atan2(vt3.y, vt3.x);
        return new Vector3(r, deg, dirDeg);
    }
    /// <summary>
    /// 获取向量
    /// </summary>
    /// <param name="dis">径向距离</param>
    /// <param name="polAngle">极角</param>
    /// <returns>2D向量</returns>
    Vector2 GetVector(float dis, float polAngle)
    {
        float x = Mathf.Cos(polAngle * Mathf.Deg2Rad) * dis;
        float y = Mathf.Sin(polAngle * Mathf.Deg2Rad) * dis;
        return new Vector2(x, y);
    }
    /// <summary>
    /// 获取向量
    /// </summary>
    /// <param name="dis">径向距离</param>
    /// <param name="polAngle">极角</param>
    /// <param name="oriAngle">方位角</param>
    /// <returns>3D向量</returns>
    Vector3 GetVector(float dis, float polAngle, float oriAngle)
    {
        float x = Mathf.Sin(polAngle * Mathf.Deg2Rad) * Mathf.Cos(oriAngle * Mathf.Deg2Rad) * dis;
        float y = Mathf.Sin(polAngle * Mathf.Deg2Rad) * Mathf.Sin(oriAngle * Mathf.Deg2Rad) * dis;
        float z = Mathf.Cos(polAngle * Mathf.Deg2Rad) * dis;
        return new Vector3(x, y, z);
    }
    /// <summary>
    ///  获取向量
    /// </summary>
    /// <param name="point">极点坐标</param>
    /// <param name="dir">径向距离</param>
    /// <param name="angle">角度</param>
    /// <returns>以极点坐标为原点2D向量</returns>
    Vector2 GetVector(Vector2 point, float dir, float angle)
    {
        float x = Mathf.Cos(angle * Mathf.Deg2Rad) * dir;
        float y = Mathf.Sin(angle * Mathf.Deg2Rad) * dir;
        return new Vector2(point.x + x, point.y + y);
    }
    /// <summary>
    /// 获取向量
    /// </summary>
    /// <param name="point">极点坐标</param>
    /// <param name="dir">径向距离</param>
    /// <param name="xoyAngle">x轴与y轴夹角</param>
    /// <param name="xozAngle">x轴与z轴夹角</param>
    /// <returns>以极点坐标为原点3D向量</returns>
    Vector3 GetVector(Vector3 point, float dir, float xoyAngle, float xozAngle)
    {
        float x = Mathf.Cos(xozAngle * Mathf.Deg2Rad) * Mathf.Cos(xoyAngle * Mathf.Deg2Rad) * dir;
        float y = Mathf.Sin(xoyAngle * Mathf.Deg2Rad) * dir;
        float z = Mathf.Sin(xozAngle * Mathf.Deg2Rad) * Mathf.Cos(xoyAngle * Mathf.Deg2Rad) * dir;
        return new Vector3(point.x + x, point.y + y, point.z + z);
    }
    /// <summary>
    /// 获取向量
    /// </summary>
    /// <param name="point">极点坐标</param>
    /// <param name="vt2">2D向量</param>
    /// <returns>以极点坐标为原点2D向量</returns>
    Vector2 GetVector(Vector2 point, Vector2 vt2)
    {
        return (vt2 + point);
    }
    /// <summary>
    /// 获取向量
    /// </summary>
    /// <param name="point">极点坐标</param>
    /// <param name="vt3">3D向量</param>
    /// <returns>以极点坐标为原点3D向量</returns>
    Vector3 GetVector(Vector3 point, Vector3 vt3)
    {
        return (vt3 + point);
    }
    /// <summary>
    /// 获取向量
    /// </summary>
    /// <param name="point">极点坐标</param>
    /// <param name="dir">径向距离</param>
    /// <param name="qt">四元数</param>
    /// <returns>以极点坐标为原点2D向量</returns>
    Vector2 GetVector(Vector2 point, float dir, Quaternion qt)
    {
        Vector3 targVt2 = new Vector2(dir, 0);
        targVt2 = qt * targVt2;
        return new Vector2(point.x + targVt2.x, point.y + targVt2.y);
    }
    /// <summary>
    /// 获取向量
    /// </summary>
    /// <param name="point">极点坐标</param>
    /// <param name="dir">径向距离</param>
    /// <param name="qt">四元数</param>
    /// <returns>以极点坐标为原点3D向量</returns>
    Vector3 GetVector(Vector3 point, float dir, Quaternion qt)
    {
        Vector3 targVt3 = qt * Vector3.right;
        targVt3 = targVt3 * dir;
        return new Vector3((point.x + targVt3.x), (point.y + targVt3.y), (point.z + targVt3.z));
    }


    # region Vector2二阶贝塞尔曲线
    /// <summary>
    /// Vector2二阶贝塞尔曲线
    /// </summary>
    /// <param name="accuracy">精度值</param>
    Vector2 QuardaticBezier2D(float accuracy)
    {
        Vector2 startPos;
        Vector2 handPos;
        Vector2 endPos;
        if (_arrayType == ArrayType.transType)
        {
            startPos = vet2Points[0].position;
            handPos = vet2Points[1].position;
            endPos = vet2Points[2].position;
        }
        else
        {
            startPos = vet2s[0];
            handPos = vet2s[1];
            endPos = vet2s[2];
        }

        Vector2 newVet1 = startPos + (handPos - startPos) * accuracy;
        Vector2 newVet2 = handPos + (endPos - handPos) * accuracy;

        return newVet1 + (newVet2 - newVet1) * accuracy;
    }
    #endregion
    #region Vector3二阶贝塞尔曲线
    /// <summary>
    /// Vector3二阶贝塞尔曲线
    /// </summary>
    /// <param name="accuracy">精度</param>
    /// <returns></returns>
    Vector3 QuardaticBezier3D(float accuracy)
    {
        Vector3 startPos;
        Vector3 handPos;
        Vector3 endPos;
        if (_arrayType == ArrayType.transType)
        {
            startPos = vet3Points[0].position;
            handPos = vet3Points[1].position;
            endPos = vet3Points[2].position;
        }
        else
        {
            startPos = vet3s[0];
            handPos = vet3s[1];
            endPos = vet3s[2];
        }
        Vector3 newVet1 = startPos + (handPos - startPos) * accuracy;
        Vector3 newVet2 = handPos + (endPos - handPos) * accuracy;

        return newVet1 + (newVet2 - newVet1) * accuracy;
    }
    #endregion

    #region Vector2三阶贝塞尔曲线
    Vector2 CubicBezier2D(float accuracy)
    {
        Vector2 startPos;
        Vector2 hand1Pos;
        Vector2 hand2Pos;
        Vector2 endPos;
        if (_arrayType == ArrayType.transType)
        {
            startPos = vet2Points[0].position;
            hand1Pos = vet2Points[1].position;
            hand2Pos = vet2Points[2].position;
            endPos = vet2Points[3].position;
        }
        else
        {
            startPos = vet2s[0];
            hand1Pos = vet2s[1];
            hand2Pos= vet2s[2];
            endPos = vet2s[3];
        }


        Vector2 newVet1 = startPos + (hand1Pos - startPos) * accuracy;
        Vector2 newVet2 = hand1Pos + (hand2Pos - hand1Pos) * accuracy;
        Vector2 newVet3 = hand2Pos + (endPos - hand2Pos) * accuracy;

        Vector2 newVet4 = newVet1 + (newVet2 - newVet1) * accuracy;
        Vector2 newVet5 = newVet2 + (newVet3 - newVet2) * accuracy;

        return newVet4 + (newVet5 - newVet4) * accuracy;
    }
    #endregion
    #region Vector3三阶贝塞尔曲线
    /// <summary>
    /// Vector3三阶贝塞尔曲线
    /// </summary>
    /// <param name="accuracy">精度</param>
    Vector3 CubicBezier3D(float accuracy)
    {
        Vector3 startPos;
        Vector3 hand1Pos;
        Vector3 hand2Pos;
        Vector3 endPos;
        if (_arrayType == ArrayType.transType)
        {
            startPos = vet3Points[0].position;
            hand1Pos = vet3Points[1].position;
            hand2Pos = vet3Points[2].position;
            endPos = vet3Points[3].position;
        }
        else
        {
            startPos = vet3s[0];
            hand1Pos = vet3s[1];
            hand2Pos = vet3s[2];
            endPos = vet3s[3];
        }

        Vector3 newVet1 = startPos + (hand1Pos - startPos) * accuracy;
        Vector3 newVet2 = hand1Pos + (hand2Pos - hand1Pos) * accuracy;
        Vector3 newVet3 = hand2Pos + (endPos - hand2Pos) * accuracy;

        Vector3 newVet4 = newVet1 + (newVet2 - newVet1) * accuracy;
        Vector3 newVet5 = newVet2 + (newVet3 - newVet2) * accuracy;

        return newVet4 + (newVet5 - newVet4) * accuracy;
    }
    #endregion

    #region Vector2四阶贝塞尔曲线
    Vector2 QuarticBezier2D(float accuracy)
    {
        Vector2 startPos;
        Vector2 hand1Pos;
        Vector2 hand2Pos;
        Vector2 hand3Pos;
        Vector2 endPos;
        if (_arrayType == ArrayType.transType)
        {
            startPos = vet2Points[0].position;
            hand1Pos = vet2Points[1].position;
            hand2Pos = vet2Points[2].position;
            hand3Pos = vet2Points[3].position;
            endPos = vet2Points[4].position;
        }
        else
        {
            startPos = vet2s[0];
            hand1Pos = vet2s[1];
            hand2Pos = vet2s[2];
            hand3Pos= vet2s[3];
            endPos = vet2s[4];
        }

        Vector2 newVet1 = startPos + (hand1Pos - startPos) * accuracy;
        Vector2 newVet2 = hand1Pos + (hand2Pos - hand1Pos) * accuracy;
        Vector2 newVet3 = hand2Pos + (hand3Pos - hand2Pos) * accuracy;
        Vector2 newVet4 = hand3Pos + (endPos - hand3Pos) * accuracy;

        Vector2 newVet5 = newVet1 + (newVet2 - newVet1) * accuracy;
        Vector2 newVet6 = newVet2 + (newVet3 - newVet2) * accuracy;
        Vector2 newVet7 = newVet3 + (newVet4 - newVet3) * accuracy;

        Vector2 newVet8 = newVet5 + (newVet6 - newVet5) * accuracy;
        Vector2 newVet9 = newVet6 + (newVet7 - newVet6) * accuracy;

        return newVet8 + (newVet9 - newVet8) * accuracy;
    }
    #endregion
    #region Vector3四阶贝塞尔曲线
    /// <summary>
    /// Vector3四阶贝塞尔曲线
    /// </summary>
    /// <param name="accuracy">精度</param>
    /// <returns></returns>
    Vector3 QuarticBezier3D(float accuracy)
    {
        Vector3 startPos;
        Vector3 hand1Pos;
        Vector3 hand2Pos;
        Vector3 hand3Pos;
        Vector3 endPos;
        if (_arrayType == ArrayType.transType)
        {
            startPos = vet3Points[0].position;
            hand1Pos = vet3Points[1].position;
            hand2Pos = vet3Points[2].position;
            hand3Pos = vet3Points[3].position;
            endPos = vet3Points[4].position;
        }
        else
        {
            startPos = vet3s[0];
            hand1Pos = vet3s[1];
            hand2Pos = vet3s[2];
            hand3Pos = vet3s[3];
            endPos = vet3s[4];
        }

        Vector3 newVet1 = startPos + (hand1Pos - startPos) * accuracy;
        Vector3 newVet2 = hand1Pos + (hand2Pos - hand1Pos) * accuracy;
        Vector3 newVet3 = hand2Pos + (hand3Pos - hand2Pos) * accuracy;
        Vector3 newVet4 = hand3Pos + (endPos - hand3Pos) * accuracy;

        Vector3 newVet5 = newVet1 + (newVet2 - newVet1) * accuracy;
        Vector3 newVet6 = newVet2 + (newVet3 - newVet2) * accuracy;
        Vector3 newVet7 = newVet3 + (newVet4 - newVet3) * accuracy;

        Vector3 newVet8 = newVet5 + (newVet6 - newVet5) * accuracy;
        Vector3 newVet9 = newVet6 + (newVet7 - newVet6) * accuracy;

        return newVet8 + (newVet9 - newVet8) * accuracy;
    }
    #endregion

    #region Vector2五阶贝塞尔曲线
    Vector2 QuinticBezier2D(float accuracy)
    {
        Vector2 startPos;
        Vector2 hand1Pos;
        Vector2 hand2Pos;
        Vector2 hand3Pos;
        Vector2 hand4Pos;
        Vector2 endPos;
        if (_arrayType == ArrayType.transType)
        {
            startPos = vet2Points[0].position;
            hand1Pos = vet2Points[1].position;
            hand2Pos = vet2Points[2].position;
            hand3Pos = vet2Points[3].position;
            hand4Pos= vet2Points[4].position;
            endPos = vet2Points[5].position;
        }
        else
        {
            startPos = vet2s[0];
            hand1Pos = vet2s[1];
            hand2Pos = vet2s[2];
            hand3Pos = vet2s[3];
            hand4Pos = vet2s[4];
            endPos = vet2s[5];
        }

        Vector2 newVet1 = startPos + (hand1Pos - startPos) * accuracy;
        Vector2 newVet2 = hand1Pos + (hand2Pos - hand1Pos) * accuracy;
        Vector2 newVet3 = hand2Pos + (hand3Pos - hand2Pos) * accuracy;
        Vector2 newVet4 = hand3Pos + (hand4Pos - hand3Pos) * accuracy;
        Vector2 newVet5 = hand4Pos + (endPos - hand4Pos) * accuracy;

        Vector2 newVet6 = newVet1 + (newVet2 - newVet1) * accuracy;
        Vector2 newVet7 = newVet2 + (newVet3 - newVet2) * accuracy;
        Vector2 newVet8 = newVet3 + (newVet4 - newVet3) * accuracy;
        Vector2 newVet9 = newVet4 + (newVet5 - newVet4) * accuracy;

        Vector2 newVet10 = newVet6 + (newVet7 - newVet6) * accuracy;
        Vector2 newVet11 = newVet7 + (newVet8 - newVet7) * accuracy;
        Vector2 newVet12 = newVet8 + (newVet9 - newVet8) * accuracy;

        Vector2 newVet13 = newVet10 + (newVet11 - newVet10) * accuracy;
        Vector2 newVet14 = newVet11 + (newVet12 - newVet11) * accuracy;

        return newVet13 + (newVet14 - newVet13) * accuracy;
    }
    #endregion
    #region Vector3五阶贝塞尔曲线
    /// <summary>
    /// Vector3五阶贝塞尔曲线
    /// </summary>
    /// <param name="accuracy">精度</param>
    /// <returns></returns>
    public Vector3 QuinticBezier3D(float accuracy)
    {
        Vector3 startPos;
        Vector3 hand1Pos;
        Vector3 hand2Pos;
        Vector3 hand3Pos;
        Vector3 hand4Pos;
        Vector3 endPos;
        if (_arrayType == ArrayType.transType)
        {
            startPos = vet3Points[0].position;
            hand1Pos = vet3Points[1].position;
            hand2Pos = vet3Points[2].position;
            hand3Pos = vet3Points[3].position;
            hand4Pos = vet3Points[4].position;
            endPos = vet3Points[5].position;
        }
        else
        {
            startPos = vet3s[0];
            hand1Pos = vet3s[1];
            hand2Pos = vet3s[2];
            hand3Pos = vet3s[3];
            hand4Pos = vet3s[4];
            endPos = vet3s[5];
        }

        Vector3 newVet1 = startPos + (hand1Pos - startPos) * accuracy;
        Vector3 newVet2 = hand1Pos + (hand2Pos - hand1Pos) * accuracy;
        Vector3 newVet3 = hand2Pos + (hand3Pos - hand2Pos) * accuracy;
        Vector3 newVet4 = hand3Pos + (hand4Pos - hand3Pos) * accuracy;
        Vector3 newVet5 = hand4Pos + (endPos - hand4Pos) * accuracy;

        Vector3 newVet6 = newVet1 + (newVet2 - newVet1) * accuracy;
        Vector3 newVet7 = newVet2 + (newVet3 - newVet2) * accuracy;
        Vector3 newVet8 = newVet3 + (newVet4 - newVet3) * accuracy;
        Vector3 newVet9 = newVet4 + (newVet5 - newVet4) * accuracy;

        Vector3 newVet10 = newVet6 + (newVet7 - newVet6) * accuracy;
        Vector3 newVet11 = newVet7 + (newVet8 - newVet7) * accuracy;
        Vector3 newVet12 = newVet8 + (newVet9 - newVet8) * accuracy;

        Vector3 newVet13 = newVet10 + (newVet11 - newVet10) * accuracy;
        Vector3 newVet14 = newVet11 + (newVet12 - newVet11) * accuracy;

        return newVet13 + (newVet14 - newVet13) * accuracy;

    }
    #endregion
    /// <summary>
    /// Vector2贝塞尔曲线公式
    /// </summary>
    public Vector2 Formula2D(Transform[] positions, float accuracy)
    {
        vet2Points = null;
        vet2Points = positions;
        _arrayType = ArrayType.transType;
        switch (vet2Points.Length)
        {
            case 3: return QuardaticBezier2D(accuracy);
            case 4: return CubicBezier2D(accuracy);
            case 5: return QuarticBezier2D(accuracy);
            case 6: return QuinticBezier2D(accuracy);
        }
        return Vector2.zero;
    }
    /// <summary>
    /// Vector3贝塞尔曲线公式
    /// </summary>
    public Vector3 Formula3D(Transform[] positions, float accuracy)
    {
        vet3Points = null;
        vet3Points = positions;
        _arrayType = ArrayType.transType;
        switch (vet3Points.Length)
        {
            case 3: return QuardaticBezier3D(accuracy);
            case 4: return CubicBezier3D(accuracy);
            case 5: return QuarticBezier3D(accuracy);
            case 6: return QuinticBezier3D(accuracy);
        }
        return Vector3.zero;
    }
    /// <summary>
    /// Vector2贝塞尔曲线公式
    /// </summary>
    public Vector2 Formula2D(Vector2[] vector2s, float accuracy)
    {
        vet2s = null;
        vet2s = vector2s;
        _arrayType = ArrayType.vectorType;
        switch (vector2s.Length)
        {
            case 3: return QuardaticBezier2D(accuracy);
            case 4: return CubicBezier2D(accuracy);
            case 5: return QuarticBezier2D(accuracy);
            case 6: return QuinticBezier2D(accuracy);
        }
        return Vector2.zero;
    }
    /// <summary>
    /// Vector3贝塞尔曲线公式
    /// </summary>
    public Vector3 Formula3D(Vector3[] vector3s, float accuracy)
    {
        vet3s = null;
        vet3s = vector3s;
        _arrayType = ArrayType.vectorType;
        switch (vector3s.Length)
        {
            case 3: return QuardaticBezier3D(accuracy);
            case 4: return CubicBezier3D(accuracy);
            case 5: return QuarticBezier3D(accuracy);
            case 6: return QuinticBezier3D(accuracy);
        }
        return Vector3.zero;
    }
}
