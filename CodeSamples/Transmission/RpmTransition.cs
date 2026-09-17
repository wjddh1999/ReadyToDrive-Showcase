// Source: Assets/Scripts/CarState.cs
// Excerpt: RPM response after an upshift
// Review sample only; surrounding state branches are omitted.

if (RPM <= 1000.0f)
    RPMChange = false;

RPM = Mathf.Lerp(RPM, 0.0f, Time.deltaTime * 2.0f);
