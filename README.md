# The Price You Pay
Game Description: 
The Price You Pay is a single-player horror adventure set in a twisted convenience store from your darkest nightmares.  
After a reckless dare turns into a deadly game, you must complete strange and dangerous tasks while avoiding the lurking terrors that stalk the aisles.

# System Requirements
Minimum Requirements:
- Operating System: Windows 10 (64-bit) or later / macOS 10.14 or later
- Processor: Intel Core i3 or equivalent
- Memory: 4 GB RAM
- Graphics: DirectX 11 compatible GPU with 1 GB VRAM
- Storage: At least 500 MB of free disk space

Recommended Requirements
- Operating System: Windows 10/11 (64-bit) / macOS 10.15 or later
- Processor: Intel Core i5 or better
- Memory: 8 GB RAM or higher
- Graphics: Dedicated GPU with 2 GB VRAM or higher
- Storage: At least 1 GB free disk space

# Controls & Gameplay Instructions
| Action             | Key                  |
| ------------------ | -------------------- |
| Move               | WASD                 |
| Jump               | Spacebar             |
| Sprint             | Shift                |
| Crouch             | Ctrl                 |
| Interact           | E                    |
| Equip              | 1                    |

# FSM Diagrams & AI Implementation Details
- Pride Monster:
  Idle → Patrol → Create Footprints → Chase Player → Jumpscare
  - Implementation: Patrols along predefined waypoints. After 8/10 footprints are cleaned by player, triggers footprint creation at Idle points.
- Greed Monster:  
  Idle → Patrol → Knock Shelf → Chase Player → Jumpscare
  - Implementation: Patrols along predefined waypoints. Knock down Lifted shelf by the player, only triggers when FSM passed by the shelf.
- Envy Monster:
  Idle → Patrol → Chase Player → Jumpscare
  - Implementation: Patrols along predefined waypoints. FSM runs really fast but is balance with long idle time.

# Game Objective
- Find the mop and clean all the footprints.
- Find the right Candy Bar amongst the pile in the convenience store.
- Lift Knocked down shelf around convenience store.
- Complete all task to Escape.
  
# Game Hack
- Horror convenience Store: Dark and Hellish theme the Mop is located at the other end of where the player is spawn behind 2 locked doors.
- The Correct Candy pile is located at the fruit Section 2nd closest to the toilet.

# Assets & Credits
- Pride Monster (Unity Asset Store): Monster Mutant 7
- Envy Monster (Unity Asset Store): Mutant monster m1
- Greed Monster (Unity Asset Store): Fantasy Goblin
- Mixamo: Use for Animation of Monsters
  https://www.mixamo.com/
- Google Fonts: Use for MainMenu UI
  https://fonts.google.com/specimen/Creepster?query=Creepster
- Chatgpt: Use for Images of Storyline
- Gemini: Use for Images of Storyline
- 11elevenlabs: AI Audio Narration for Start and End
