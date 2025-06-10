# Unity Dialogue Control System

This is a lightweight, editor-integrated **Dialogue Control System** for Unity, designed for quick deployment and intuitive usage. It includes both runtime functionality and a custom Unity Editor tool to manage dialogue content efficiently.

> **Note**: This system is a **personal use, experimental product**. For questions or detailed instructions on how to use it, feel free to contact:  


---

## Features

- **Supports up to 4 portraits simultaneously** with an automatic portrait placement system.
- **Manual portrait assignment** supported per dialogue line when needed.
- **Localized dialogue line and options** management, powered by the **Unity Localization Package**.
- **Custom DialogueData Editor Window**:
  - Detects available localized string tables.
  - Automatically creates and updates localized entries when editing dialogue.
  - Allows real-time locale switching via a toolbar dropdown.

---

## Requirements

- **Unity 2021 or later**
- **Unity Localization Package** (Install via Package Manager)

---

## Getting Started

1. **Install the Unity Localization package**:
   - Go to `Window > Package Manager`
   - Search for `Localization` and install it.

2. **Open the Dialogue Editor**:
   - Menu: `Tools > Dialogue Editor`

3. **Load your assets**:
   - Assign a `DialogueData` asset to begin editing.
   - Optionally assign an `ActorDatabase` asset for actor name previews.

4. **Create and edit dialogue**:
   - Add lines, actors, options, jump targets, and localized text.
   - Supports both automatic and manual portrait assignments.

5. **Localization**:
   - Automatically generates string tables and entries when needed.
   - Localized entries are synced with Unity's Localization system.

6. **Change Editor Locale**:
   - Use the top-right dropdown to switch the current editing locale.

## Notes

- Localization keys are automatically generated to avoid conflicts.
- Avoid editing localization tables outside the editor unless necessary.
- Each dialogue line and its options are linked to a unique key.
- Portrait assignment can be overridden line-by-line using manual assignment.

---

## Potential Future Features

- Audio clip support per line
- Condition-based branching
- Runtime preview and playtest mode

---

## License

MIT License. Free for personal and commercial use.  
This project is experimental and maintained personally.

---

**Contact**: zackyang421@gmail.com for assistance or questions.