using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TeamARandomChat : MonoBehaviour
{

    public TextMeshProUGUI commentText;  // Text component to display comments
    private string[] comments = new string[] {
       // Greetings
        "Hey there, farmer!",
        "Good luck, you’ll need it!",
        "Hello, slowpoke!",
        "Ready to chase me? Let's go!",
        "Hi, farmer! Hope you're ready!",
        "Hello from the other side!",
        "Good day to get outrun, right?",
        "Hey, just a friendly deer here!",
        "What's up, farmer? Let's play!",
        "Morning! Or is it afternoon? Time flies when you're fast!",
        
        // Taunts
        "Catch me if you can, farmer!",
        "Too slow, you'll never catch me!",
        "Is that all you've got? I'm just getting started!",
        "I could run circles around you!",
        "Look behind you, I'm right there!",
        "You call that running? I'm barely breaking a sweat!",
        "Come on, catch up! Oh wait, you can't!",
        "I can do this all day!",
        "You're too slow for me, try harder!",
        "Not even close, farmer!",
        "I'm already gone, better luck next time!",
        "Think you can catch me? Think again!",
        "You almost had me... not!",
        "You look tired, need a break?",
        "Better luck next time, slowpoke!",
        "I'm too quick for you!",
        "You can't outsmart a deer!",
        "Give up yet? You should!",
        "I'm right behind you... or am I?",
        "You’re going the wrong way!",
        "Can’t catch what you can’t see!",
        "You’re making this too easy!",
        "I’ve seen turtles move faster!",
        "Is this a chase or a walk in the park?",
        "Good try, but not good enough!",
        "Getting warmer... nope, just kidding!",
        "Almost had me there! Just kidding!",
        "Are you sure you're a farmer? You run like a snail!",
        "I'm practically standing still, and you still can't catch me!",
        "Guess what? I'm still here, and you're still too slow!",
        "I can do this all day, can you?"
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