#include "Demo.h"

Demo::Demo(const SpawnParams& params)
    : Script(params)
{
    // Enable ticking OnUpdate function
    _tickUpdate = true;
}

void Demo::OnEnable()
{
    // Here you can add code that needs to be called when script is enabled (eg. register for events)
}

void Demo::OnDisable()
{
    // Here you can add code that needs to be called when script is disabled (eg. unregister from events)
}

void Demo::OnUpdate()
{
    // Here you can add code that needs to be called every frame
}
