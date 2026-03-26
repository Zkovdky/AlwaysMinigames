# AlwaysMinigames

A LabAPI plugin for SCP:SL that interacts with the AutoEvent minigame plugin. 

Essentially, it monitors the current game state: if a minigame is in progress, it waits for it to end and then automatically starts a new voting process using the `.v` command and the auto-event ID.

> [!IMPORTANT]
> **No Configuration File:** This plugin was designed for a specific project and does not include a `.yml` config. To change its behavior (e.g., voting logic, or timing), you must modify the source code directly or add config variables.

## How to Use
This plugin is not provided as a pre-compiled binary. To use it, you need to download the source code and build the plugin yourself.

## Dependencies
This plugin specifically requires the **LabAPI remake (fork)** of AutoEvent to function correctly:
* **[AutoEvent LabAPI Fork by MedveMarci](https://github.com/MedveMarci/AutoEvent)**

## Credits & Links
* **MedveMarci**: [Creator of the LabAPI AutoEvent fork](https://github.com/MedveMarci) (Primary dependency).
* **risottoman (KoT0XleB)**: [Original creator of the AutoEvent plugin](https://github.com/RisottoMan).
* **Original Repository**: [RisottoMan/AutoEvent](https://github.com/RisottoMan/AutoEvent).

## License
This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.
