using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TeamBRandomChat : MonoBehaviour
{

    public TextMeshProUGUI commentText;  // Text component to display comments
    private string[] comments = new string[] {
       "You can run, but you can't hide forever!",
        "I know you're around here somewhere, little deer!",
        "I’m getting closer… I can smell your fear!",
        "Come out, come out, wherever you are!",
        "You think you’re safe? Think again!",
        "I’m not leaving until I catch you!",
        "Your hiding spots are no match for me!",
        "Ready or not, here I come!",
        "I’ll find you, it’s only a matter of time!",
        "You can’t stay hidden forever!",
        "I’ll catch you, just you wait!",
        "You’re making this too easy, deer!",
        "I’m on your trail, won’t be long now!",
        "You might as well give up, I’m too good!",
        "I know all the best hiding spots, you’re doomed!",
        "Come out now and I might go easy on you!",
        "I’m a master tracker, you don’t stand a chance!",
        "I’m hot on your trail, deer!",
        "Running won’t save you, I’ll catch you!",
        "I’m getting warmer…!",
        "Are you scared? You should be!",
        "I’ve got you cornered!",
        "Just give up now, it’s inevitable!",
        "You’re not as clever as you think, deer!",
        "I’m right behind you… or am I?",
        "You can’t outsmart me, I’m the best!",
        "I’m closing in on you, deer!",
        "You’re running out of time!",
        "I’m faster than I look, deer!",
        "Think you can outsmart me? Let’s see about that!"
    };
    public void SelectRandomConmment()
    {
        // Kiểm tra xem danh sách tên có phần tử hay không
        if (comments.Length > 0)
        {
            // Chọn một tên ngẫu nhiên từ danh sách
            string randomComment = comments[Random.Range(0, comments.Length)];

            // Thay đổi text của biến đối tượng public nameText
            if (commentText != null)
            {
                commentText.text = randomComment;
            }
            else
            {
                Debug.LogWarning("Bạn chưa gán đối tượng TextMeshPro cho biến nameText.");
            }
        }
        else
        {
            Debug.LogWarning("Danh sách tên trống. Vui lòng thêm các tên vào danh sách.");
        }
    }
}