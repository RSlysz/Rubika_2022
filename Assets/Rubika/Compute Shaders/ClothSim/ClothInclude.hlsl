struct Point
{
    float3 position, prevPosition, normal;
    int locked;
    int connectionsCounts;
    int neighborUp;
    int neighborDown;
    int neighborLeft;
    int neighborRight;
};