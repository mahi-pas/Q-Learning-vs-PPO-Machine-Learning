# Q Learning vs PPO Machine Learning
Play demo here: https://rainyelephant.itch.io/ai-demo-q-learning-vs-ppo-machine-learning

## Features:
- Q Learning fully implemented from scratch
  - Ability to customize these settings:
    - number of episodes
    - number of episode steps
    - exploration probability
    - minimum exploration probability
    - exploration decay
    - alpha
    - gamma
    - number of states
    - number of actions
    - seconds between turns (should be 0 in practice, but useful to slow down for debugging)
  - All user must do is inherit from QAgent and define these two functions:
    - `GetNextState(int currentState, int action)`
      - Within this function user must execute the action and return the resulting state
    - `GetReward(Vector2 oldPosition, Vector2 newPosition, int nextState)`
      - Return a reward based on state or based on position
      - If more is required in your implementation, edit the header to pass in more data
  - User can also overload certain important functions:
    - `Start()`
    - `Update()`
    - `Iterate()`
    - `OnEpisodeBegin()`
    - `OnEpisodeEnd()`
- 2 sample Q learning implementations
  - 4 direction move to goal
  - Tank/Spaceship controls move to goal
- Live Q Learning training screen that displays Q Table and training data in real time
- Ability to graph values and track performance by editing values under "Data Science"
- Ability to change timescale
- PPO ML Agent using Unity ML Agents Package for same scenarios Q Learning uses
- This project is open source, feel free to use it in your projects!
    
