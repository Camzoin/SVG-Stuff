using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class URLSaver : MonoBehaviour
{
    public string artName;
    public string shortURL;

    private string formUrl = "https://docs.google.com/forms/d/e/1FAIpQLSdVQ7V5PHwWunnw6Qrjj1HhLtmw7OpgN-WgGgfmSyzmrtAFEQ/formResponse";


    [ContextMenu("CompileInfoPage")]
    public void SubmitFeedback()
    {
        StartCoroutine(Post(artName, shortURL));
    }

    private IEnumerator Post(string positive, string negative)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.1179115825", positive);
        form.AddField("entry.335327588", negative);


        using (UnityWebRequest www = UnityWebRequest.Post(formUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Feedback submitted successfully.");
            }
            else
            {
                Debug.LogError("Error in feedback submission: " + www.error);
            }
        }
    }
}