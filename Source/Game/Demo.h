#pragma once

#include "Engine/Scripting/Script.h"

API_CLASS() class GAME_API Demo : public Script
{
API_AUTO_SERIALIZATION();
DECLARE_SCRIPTING_TYPE(Demo);

    // [Script]
    void OnEnable() override;
    void OnDisable() override;
    void OnUpdate() override;
};
