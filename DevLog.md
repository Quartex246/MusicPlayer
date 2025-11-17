# Music Player Dev Log



## Introduction

A basic music player developed in C# in Winform.

The application is created to finish a course about Object-oriented programming.



## 250909

I've decided to write logs to record the development. It should be really useful.

Currently, the UI has been properly placed. I've made a mistake - Not making a graph before development. This has lead to a disorder problem.

 

## 251109

Welp, it has been a long time. Basically like 2 months has passed, and basically no progress.

I've been working on something else, partially prepare for an exam, but mostly wasting time.

Still, new progress has been made and here's the record.



Two months ago I said the disorder problem on UI, that is probably fixed. The problem is basically didn't consider all the possible functions and some buttons had to be added afterwards. 



Currently, there are two major problems:

- **Basic music play / pause function**
- **Playlist function**



From a reminder of my friend, I realized it was a mistake to prioritize the playlist function instead the control function, which is also a reason of low efficiency. So yesterday I started to work on the basic controls.



According to a search engine, play / pause function can be achieved by following code

`axWindowsMediaPlayer1.URL = @"C:\Music\example.mp3"; // Set file directory
axWindowsMediaPlayer1.Ctlcontrols.play(); // Play music`



However, it failed. The file won't play after hitting the play / pause, nothing happens. Which is why I asked for AI to help me what happened.



According to AI, the directory seems to a little problem

> The problem is you set an incorrect music path in `PlayMusic` method. You directly assigned a folder path to `wplayer.URL`, instead of a specific music file path.

Then it suggest me to add music by playlist instead of writing a fixed path.



Yep, we're back to the beginning.



But instead of giving useless advices, this time it gave me a more specific approach.











