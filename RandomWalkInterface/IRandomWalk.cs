using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RandomWalkFramework.RandomWalkInterface {

	public delegate void TransitionEvent<TState>(IRandomWalk<TState> sampler, TState previous, TState current, decimal weight);



	public interface IRandomWalk<TVertex> {

		decimal TimeIncrement { get; }
		KeyValuePair<string, string> Name { get; }
		event TransitionEvent<TVertex> Step;
		void Initialize();
		TVertex NextSample();

		void Terminate();
		event EventHandler Terminated;

		decimal TotalSteps { get; }
		ulong DiscreetSteps { get; }
		TVertex CurrentState { get; }
		TVertex PreviousState { get; }
		TVertex InitialState { get; }

		int GetAdjacentTransitionCount(TVertex state);
		TVertex GetAdjacentTransition(TVertex state, int index);
		decimal GetStateWeight(TVertex state);
		decimal GetTransitionWeight(TVertex source, TVertex target);
	}


}
