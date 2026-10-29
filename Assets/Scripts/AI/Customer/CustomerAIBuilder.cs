using System.Collections.Generic;
using UnityEngine;

public class CustomerAIBuilder
{
    public bool BTActivated = false;
    public bool FSMActivated = false;

    public CustomerFiniteStateMachine FSM = new CustomerFiniteStateMachine();
    public CustomerBehaviourTree BT = new CustomerBehaviourTree();

    public class CustomerFiniteStateMachine
    {
        public FSMBaseState currentState;

        public EnterStoreState enterStoreState = new EnterStoreState();
        public NothingState nothingState = new NothingState();
        public StandInLineState standInLineState = new StandInLineState();
        public GoToLineState goToLineState = new GoToLineState();
        public FirstInLineState firstInLineState = new FirstInLineState();
        public ExitStoreState exitStoreState = new ExitStoreState();

        public void SwitchState(FSMBaseState state, CustomerManager agent)
        {
            currentState = state;
            state.EnterState(agent);
        }
    }

    public class CustomerBehaviourTree
    {

        public BTNode rootNode;

        //------Idle scripts-------------

        IdleConditions idleConditions = new IdleConditions();
        Idle idle = new Idle();

        //------GetHitState scripts ------

        GetHitConditions getHitConditions = new GetHitConditions();
        FallBackwardConditions fallBackwardConditions = new FallBackwardConditions();
        public FallBackward fallBackward = new FallBackward();
        FallForwardConditions fallForwardConditions = new FallForwardConditions();
        public FallForward fallForward = new FallForward();

        //------SabotageState scripts -------

        SabotageConditions sabotageConditions = new SabotageConditions();
        GoToStallConditions goToStallConditions = new GoToStallConditions();
        GoToStall goToStall = new GoToStall();
        HitFoodConditions hitFoodConditions = new HitFoodConditions();
        public HitFood hitFood = new HitFood();
        TauntPlayerConditions tauntPlayerConditions = new TauntPlayerConditions();
        public TauntPlayer tauntPlayer = new TauntPlayer();


        //------SearchForPlayerState scripts ------

        SearchConditions searchConditions = new SearchConditions();
        ChasePlayerConditions chasePlayerConditions = new ChasePlayerConditions();
        ChasePlayer chasePlayer = new ChasePlayer();
        FollowPlayerConditions followPlayerConditions = new FollowPlayerConditions();
        FollowPlayer followPlayer = new FollowPlayer();
        IdleStareConditions idleStareConditions = new IdleStareConditions();
        public IdleStare idleStare = new IdleStare();


        //------ConfusedState scripts ------

        ConfusedConditions confusedConditions = new ConfusedConditions();
        PatroleAisleConditions patroleAisleConditions = new PatroleAisleConditions();
        public PatroleAisle patroleAisle = new PatroleAisle();
        CheckWrongShelfConditions checkWrongShelfConditions = new CheckWrongShelfConditions();
        public CheckWrongShelf checkWrongShelf = new CheckWrongShelf();


        //------ShelfState scripts ------

        GrabItemConditions grabItemConditions = new GrabItemConditions();
        GoToShelfConditions goToShelfConditions = new GoToShelfConditions();
        GoToShelf goToShelf = new GoToShelf();
        PickGoodsConditions pickGoodsConditions = new PickGoodsConditions();
        public PickGoods pickGoods = new PickGoods();

        public void ConstructBT()
        {

            //IDLE STATE BRANCH

            Sequence standIdle = new Sequence(new List<BTNode>() { idleConditions, idle });

            //GETHIT STATE BRANCH

            Sequence fallBackwardState = new Sequence(new List<BTNode>() { fallBackwardConditions, fallBackward });
            Sequence fallForwardState = new Sequence(new List<BTNode>() { fallForwardConditions, fallForward });

            Selector fallingDirectionBehaviour = new Selector(new List<BTNode> { fallBackwardState, fallForwardState });
            Sequence getHit = new Sequence(new List<BTNode>() { getHitConditions, fallingDirectionBehaviour });


            //SABOTAGE BRANCH

            Sequence goToStallState = new Sequence(new List<BTNode>() { goToStallConditions, goToStall });
            Sequence hitFoodState = new Sequence(new List<BTNode>() { hitFoodConditions, hitFood });
            Sequence tauntPlayerState = new Sequence(new List<BTNode>() { tauntPlayerConditions, tauntPlayer });

            Selector sabotageBehaviour = new Selector(new List<BTNode>() { goToStallState, hitFoodState, tauntPlayerState });
            Sequence sabotage = new Sequence(new List<BTNode>() { sabotageConditions, sabotageBehaviour });


            //SEARCHFORPLAYER STATE BRANCH

            Sequence chasePlayerState = new Sequence(new List<BTNode>() { chasePlayerConditions, chasePlayer });
            Sequence followPlayerState = new Sequence(new List<BTNode>() { followPlayerConditions, followPlayer });
            Sequence idleStareState = new Sequence(new List<BTNode>() { idleStareConditions, idleStare });

            Selector playerSpottedBehaviour = new Selector(new List<BTNode> { chasePlayerState, followPlayerState, idleStareState });
            Sequence searchForPlayer = new Sequence(new List<BTNode>() { searchConditions, playerSpottedBehaviour });


            //CONFUSED STATE BRANCH

            Sequence patroleAisleState = new Sequence(new List<BTNode>() { patroleAisleConditions, patroleAisle });
            Sequence checkWrongShelfState = new Sequence(new List<BTNode>() { checkWrongShelfConditions, checkWrongShelf });

            Selector confusedBehaviour = new Selector(new List<BTNode> { patroleAisleState, checkWrongShelfState });
            Sequence confused = new Sequence(new List<BTNode>() { confusedConditions, confusedBehaviour });


            //SHELF STATE BRANCH
            Sequence goToShelfState = new Sequence(new List<BTNode>() { goToShelfConditions, goToShelf });
            Sequence pickGoodsState = new Sequence(new List<BTNode>() { pickGoodsConditions, pickGoods });


            Selector grabItemBehaviour = new Selector(new List<BTNode> { goToShelfState, pickGoodsState });
            Sequence grabItem = new Sequence(new List<BTNode>() { grabItemConditions, grabItemBehaviour });



            rootNode = new Selector(new List<BTNode> { standIdle, getHit, sabotage, searchForPlayer, confused, grabItem });
        }

    }
}
