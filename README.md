# Capicu Domino

A Unity-based mobile Domino game for iOS and Android.

## Overview

Capicu Domino is a cross-platform mobile game built in Unity, featuring online multiplayer gameplay (via Nakama), in-app purchases, ads integration, and social login support.

## Tech Stack

- **Engine:** Unity
- **Platforms:** iOS, Android
- **Backend / Multiplayer:** Nakama
- **Monetization:** Unity IAP, Google Mobile Ads
- **Authentication:** Apple Sign In, Google Sign-In, Facebook, Guest login

## Key Features

- Real-time multiplayer Domino gameplay
- Multiple sign-in options (Apple, Google, Facebook, Guest)
- In-app purchases
- Ad integration
- Native sharing and gallery access

## Project Structure

- `Assets/Scripts/Controller Scripts/Login Controller/` — Authentication providers (Apple, Google, Facebook, Guest)
- `Assets/Scripts/Controller Scripts/Global.cs` — Shared constants and configuration
- `Assets/Entitites/Player/` — Player data models
- `Assets/Scripts/Nakama Scripts/` — Multiplayer/networking logic
- `Assets/Plugins/Android/` — Android Gradle build configuration
- `Assets/AppleSignInUnity/` — Apple Sign In plugin

## Getting Started

1. Clone the repository.
2. Open the project in Unity (see `ProjectSettings/ProjectVersion.txt` for the required Unity version).
3. For Android builds, ensure `google-services.json` is present under `Assets/`.
4. Build and run via **File > Build Settings** for your target platform (iOS/Android).

## Contact & Support

For questions, bug reports, or future maintenance requests regarding this project, please reach out:

**Mohammad Hunain**
- 📧 Email: [mohammadhunain83@gmail.com](mailto:mohammadhunain83@gmail.com)
- 📱 Phone: +92 315 8327536

## License

Proprietary — All rights reserved.
