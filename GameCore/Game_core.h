#pragma once

#ifdef GAMECORE_EXPORTS
#define GAME_API __declspec(dllexport)
#else
#define GAME_API __declspec(dllimport)
#endif

#ifdef __cplusplus
extern "C" {
#endif

    // ===== 游戏生命周期 =====
    GAME_API void Game_Init();
    GAME_API void Game_NextDay();

    // ===== 状态查询 =====
    GAME_API int Game_GetDay();
    GAME_API int Game_GetHealth();
    GAME_API int Game_GetFood();
    GAME_API int Game_GetWater();
    GAME_API int Game_IsAlive();

#ifdef __cplusplus
}
#endif
