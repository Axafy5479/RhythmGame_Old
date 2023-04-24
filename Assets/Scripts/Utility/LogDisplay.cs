using UnityEngine;
using System.Collections.Generic;

public class LogDisplay : MonoBehaviour
{
    private const int LOG_MAX = 10;
    private Queue<string> logStack = new Queue<string>(LOG_MAX);

    void Awake()
    {
        Application.logMessageReceived += LogCallback;  // ���O�������o���ꂽ���̃R�[���o�b�N�ݒ�
        Debug.LogWarning("hoge");

    }

    /// <summary>
    /// ���O���擾����R�[���o�b�N
    /// </summary>
    /// <param name="condition">���b�Z�[�W</param>
    /// <param name="stackTrace">�R�[���X�^�b�N</param>
    /// <param name="type">���O�̎��</param>
    public void LogCallback(string condition, string stackTrace, LogType type)
    {
        // �ʏ탍�O�܂ŕ\������Ǝז��Ȃ̂Ŗ���
        if (type == LogType.Log)
            return;

        string trace = null;
        string color = null;

        switch (type)
        {
            case LogType.Warning:
                // UnityEngine.Debug.XXX�̏璷�ȏ����Ƃ�
                trace = stackTrace.Remove(0, (stackTrace.IndexOf("\n") + 1));
                color = "yellow";
                break;
            case LogType.Error:
            case LogType.Assert:
                // UnityEngine.Debug.XXX�̏璷�ȏ����Ƃ�
                trace = stackTrace.Remove(0, (stackTrace.IndexOf("\n") + 1));
                color = "red";
                break;
            case LogType.Exception:
                trace = stackTrace;
                color = "red";
                break;
        }

        // ���O�̍s����
        if (this.logStack.Count == LOG_MAX)
            this.logStack.Dequeue();

        string message = string.Format("<color={0}>{1}</color> <color=white>on {2}</color>", color, condition, trace);
        this.logStack.Enqueue(message);

    }

    /// <summary>
    /// �G���[���O�\��
    /// </summary>
    void OnGUI()
    {
        if (this.logStack == null || this.logStack.Count == 0)
            return;

        // �\���̈�͔C��
        float space = 16f;
        float height = 300f;
        Rect drawArea = new Rect(space, (float)Screen.height - height - space, (float)Screen.width * 0.5f, height);
        GUI.Box(drawArea, "");

        GUILayout.BeginArea(drawArea);
        {
            GUIStyle style = new GUIStyle();
            style.wordWrap = true;
            foreach (string log in logStack)
                GUILayout.Label(log, style);
        }
        GUILayout.EndArea();
    }
}
