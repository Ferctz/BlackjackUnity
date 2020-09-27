# Blackjack Developer Coding Assignment

**Features Implemented**

* Single-player game (dealer and one player at minimum)
* Basic Blackjack rules as described here: https://en.wikipedia.org/wiki/Blackjack#Rules
* The player should have cash and be able to bet according to the rules here: https://en.wikipedia.org/wiki/Blackjack#Rules_of_play_at_casinos (You can simplify these, as long as the basic bet/reward structure is in place)
* The ability to play multiple rounds with a persistent cash pool
* The ability to save game state and restore it after restarting (due to time constraints, only cash is reloaded however player state is being saved)
* Customizable game settings (min bet amount & player starting cash amount)
* Create a fully functional game of Blackjack from scratch using only standard classes and common frameworks.

**Nice-to-Haves Implemented**

* Between 2-6 user controlled players at the table where players can enter/leave the table at any time (all controlled locally) (leaving is coming soon)
* Animations! (player turn animation)

**Outline of Code Architecture**

This project implements a finite state machine to drive the navigation between the states inside the game of Blackjack. Specifically, BlackjackManager.cs implements this state machine and the states are: Shuffling, Betting, Dealing, Playing, and Results.

This approach came to be due to the debugging power of a state machine. Not only does it print out a debug message for every new state traversed, methods for enter/update/leave per state are explicitly assigned.

When thinking about a larger scope, this game flow works great if this game became networked. BlackjackManager.cs is what you'd have working as server code and you would only need to set up a few RPC calls to sync player states on client machines.

Player.cs and Dealer.cs both inherit from Scorer.cs. Each scorer has a ScoreData struct which tracks all hands per play, as well as their associated bet value. A card's value is calculated inside Scorer.cs.

Data structure for cards that are part of the deck is handled by a stack. 416 CardData structs are created, randomized and added to the stack. This is done inside Deck.cs.

Instances of card prefab are pooled and reused on consecutive plays. CardPool.cs inherits from the base pool class Pool.cs.

Hand splitting is accounted for as scorers can have multiple hands. Checking for hand splitting logic would go inside of BlackjackManager.UpdateDealingState() when a scorer is dealt their second hand.

GameSession.cs contains the serializale struct for saving a game session. Session data inside is saved out to persistent data path. Sessions are loaded on Start() of BlackjackManager.cs and saved whenever OnApplicationQuit() is fired.

**How To Play**

Pull down the repo, open the project using Unity 2018.4.21f1, open up Game scene under Assets/Scene/Game.unity, and hit play.