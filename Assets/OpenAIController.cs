using OpenAI_API;
using OpenAI_API.Models;
using OpenAI_API.Chat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;



public class OpenAIController : MonoBehaviour
{

    public TMP_Text textField;
    public TMP_InputField inputField;
    public Button okButton;

    private OpenAIAPI api;
    private List<ChatMessage> messages;

    // Start is called before the first frame update
    void Start()
    {
        api = new OpenAIAPI(Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.User));
        StartConversation();
        okButton.onClick.AddListener(() => GetResponse());
    }

    
    private void StartConversation()
    {

        messages = new List<ChatMessage> {
            new ChatMessage(ChatMessageRole.System, "You are Julius Caesar, you will attempt to emulate him as best as possible using his writings in De Bello and De bello civili. You will never break character and always pretend you are Julius Caesar. Reply with relativley short but informative answers up to 6 sentences long.")
        };
        
        inputField.text = "";
        string startString = "You are in the presence of Julius Caesar, you may ask him anything.";
        textField.text = startString;
        Debug.Log(startString);

    }
    
    private async void GetResponse()
    {
        if (inputField.text.Length < 1)
        {
            return;
        }

        
        //Disable OK Button
        okButton.enabled = false;


        //Fill the user message from the input field
        ChatMessage userMessage = new ChatMessage();
        userMessage.Role = ChatMessageRole.User;
        userMessage.Content = inputField.text;
        if (userMessage.Content.Length > 100)
        {
            // Limit Message to 100 characters
            userMessage.Content = userMessage.Content.Substring(0, 100);

        }
        Debug.Log(string.Format("{0}: {1}", userMessage.rawRole, userMessage.Content));

        //Add the user message to the list of messages
        messages.Add(userMessage);

        textField.text = string.Format("You: {0}", userMessage.Content);

        inputField.text = "";

        //Get the response from the API
        var chatResult = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            Model = Model.ChatGPTTurbo,
            Temperature = .5,
            MaxTokens = 300,
            Messages = messages
        });

        ChatMessage responseMessage = new ChatMessage();
        responseMessage.Role = chatResult.Choices[0].Message.Role;
        responseMessage.Content = chatResult.Choices[0].Message.Content;
        Debug.Log(string.Format("{0}: {1}", userMessage.rawRole, responseMessage.Content));

        messages.Add(responseMessage);

        textField.text = string.Format("You: {0}\n\nJulius Caesar :{1}", userMessage.Content, responseMessage.Content);

        okButton.enabled = true;
        
    }


}
