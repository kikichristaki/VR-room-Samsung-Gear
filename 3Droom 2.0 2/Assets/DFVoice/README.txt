Thank you for choosing DaikonForge technology to power your game.
When using DaikonForge middleware in a game or application, we allow you to: 
	1.) Display one of the included splash screens on game startup.
	2.) Display a small logo badge somewhere in your game
	3.) Add a section on DaikonForge products to your credits

However, these are all totally optional - if you don't want to, it's entirely up to you.

SPLASH SCREEN
Two such splash screens are included - one with a white background, and one with a black background.
The splash screen displays a “Powered By DaikonForge” logo. In addition to this, it includes placeholder copyright text at the bottom of the screen.
This placeholder should be replaced with text of the following form:

	This software application includes [used products here]
	Copyright © 2014 DaikonForge All Rights Reserved

Where products are separated as such: “X, Y, and Z”

LOGO BADGE
A selection of DaikonForge logo badges can be found in the Badge subfolder of the DFVoice folder. This can be placed anywhere of your choosing in the game.

END CREDITS
When mentioning DaikonForge products in your credits, it should take the following form:

	POWERED BY DAIKONFORGE TECHNOLOGY

Followed by a list of plugins used. For instance, if your game used DFVoice, it would appear as follows:

	POWERED BY DAIKONFORGE TECHNOLOGY
	- DFVoice

EXPERIMENTAL OPUS SUPPORT
While the default codec is a managed port of Speex called NSpeex, DFVoice includes experimental support for the successor to Speex, called Opus.
To enable this support, import the included "OpusSupport.unitypackage", and reboot Unity in order to enable unsafe compilation.
Then, override GetCodec() to the following:

public override IAudioCodec GetCodec()
{
    AudioUtils.FrequencyProvider = new OpusCodec.FrequencyProvider();
	return new OpusCodec( 64000 );
}

You'll also probably want to change the chunk size of the Microphone component for best results - I recommend a chunk size of 960 (this is the frame size of the Opus codec internally).

The Opus codec wrapper takes the desired bitrate as a parameter. 64 kb/s is a good starting point - you can adjust it up if you need more quality, or adjust it down for lower bandwidth.
Included are two DLLs - one is a 32-bit version of the native Opus DLL, the other is a 64-bit version. These are Windows only. If you would like to get Opus working on other platforms,
you will need to download the Opus source code and compile it for your platform of choice.