#include "Game_core.h"
#include <stdlib.h>
#include <time.h>

/* =========================
   内部游戏状态（不对外）
   ========================= */
static int g_day;
static int g_health;
static int g_food;
static int g_water;

/* =========================
   内部工具函数
   ========================= */
static void init_player() {
    g_health = 100;
}

static void init_resources() {
    g_food = 10;
    g_water = 10;
}

static void consume_resources() {
    g_food--;
    g_water--;

    if (g_food < 0) g_food = 0;
    if (g_water < 0) g_water = 0;
}

static void trigger_random_event() {
    int r = rand() % 5;

    switch (r) {
    case 0: g_health -= 5; break;   // 辐射
    case 1: g_health -= 10; break;  // 入侵
    case 2: g_health -= 3; break;   // 设备故障
    case 3: g_food -= 2; g_water -= 2; break; // 资源泄露
    default: break; // 平静的一天
    }

    if (g_health < 0) g_health = 0;
    if (g_food < 0) g_food = 0;
    if (g_water < 0) g_water = 0;
}

/* =========================
   对外 API 实现
   ========================= */
GAME_API void Game_Init() {
    srand((unsigned)time(NULL));
    g_day = 1;
    init_player();
    init_resources();
}

GAME_API void Game_NextDay() {
    if (g_health <= 0)
        return;

    g_day++;

    consume_resources();
    trigger_random_event();

    // 饥渴惩罚
    if (g_food == 0 || g_water == 0) {
        g_health -= 10;
        if (g_health < 0) g_health = 0;
    }
}

GAME_API int Game_GetDay() {
    return g_day;
}

GAME_API int Game_GetHealth() {
    return g_health;
}

GAME_API int Game_GetFood() {
    return g_food;
}

GAME_API int Game_GetWater() {
    return g_water;
}

GAME_API int Game_IsAlive() {
    return g_health > 0 ? 1 : 0;
}
