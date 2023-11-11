void Steam_Check()
{
    if (h_steam_dll = LoadLibraryA("steam.dll"))
    {
        if (SteamIsAppSubscribed = GetProcAddress(h_steam_dll, "SteamIsAppSubscribed"))
        {
            if (SteamStartup = GetProcAddress(h_steam_dll, "SteamStartup"))
            {
                if (SteamCleanup = GetProcAddress(h_steam_dll, "SteamCleanup");)
                {
                    if (SteamStartup(14, error))
                    {
                        SteamStartup_done = 1;

                        // 3920 is the game id.

                        if (SteamIsAppSubscribed(3920, &IsAppSubscribed, &IsSubscriptionPending, error))
                        {
                            if (!IsAppSubscribed)
                            {
                                wsprintfA(err, "You do not have a valid Steam subscription for this application [AppId=%d].", 3920);
                                steam_error_text = err;
                                steam_error_code = 5;
                            }
                        }
                        else
                        {
                            steam_error_text = "Problem checking Steam application.";
                            steam_error_code = 4;
                        }
                    }
                    else
                    {
                        steam_error_text = "Problem starting up Steam.";
                        steam_error_code = 3;
                    }
                }
                else
                {
                    steam_error_text = "Problem accessing Steam.";
                    steam_error_code = 2;
                }
            }
            else
            {
                steam_error_text = "Problem accessing Steam.";
                steam_error_code = 2;
            }
        }
        else
        {
            steam_error_text = "Problem accessing Steam.";
            steam_error_code = 2;
        }
    }
    else
    {
        steam_error_text = "Failed to find Steam.";
        steam_error_code = 1;
    }

    // Steam cleanup.
    if (SteamStartup_done)
        SteamCleanup(error);

    // Unload steam library.
    if (h_steam_dll)
        FreeLibrary(h_steam_dll);

    // If no Steam subscription then show error message.
    if (!IsAppSubscribed)
    {
        if (steam_error_text)
        {
            MessageBoxA(0, steam_error_text, "Application error", 16);
            open_steam_webpage();
        }
        ExitProcess(-1);
    }
}