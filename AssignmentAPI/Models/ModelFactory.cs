﻿using AssignmentAPI.Data.Entities;
using AssignmentAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssignmentAPI.Models
{
    public class ModelFactory
    {
        private IAssignmentData _data;

        public ModelFactory(IAssignmentData data)
        {
            _data = data;
        }
        public PlayerEntity Parse(PlayerModel playerModel)
        {
            return new PlayerEntity { Name = playerModel.Name, YearOfBirth = playerModel.YearOfBirth };
        }

        public MatchEntity Parse(MatchModel matchModel)
        {
            return new MatchEntity { MatchDateTime = DateTime.Parse(matchModel.MatchDateTime), MatchTitle = matchModel.MatchTitle };
        }

        public async Task<MatchPlayerEntity> Parse(int matchId, int playerID)
        {
            try
            {
                var match = await _data.GetMatch(matchId);
                if (match == null)
                    throw new Exception("No such match exists.");

                var player =  await _data.GetPlayer(playerID);
                if (player == null)
                    throw new Exception("No such player exists");

                var matchPlayers = _data.GetMatchPlayersInMatch(matchId);

                if (matchPlayers.Any(x => x.Player.PlayerID == playerID))
                {
                    throw new Exception("Player already exists in the match");
                }

                return new MatchPlayerEntity { Match = match, Player = player };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IQueryable<MatchPlayerModel> Create(IQueryable<MatchPlayerEntity> matchPlayer)
        {
            return matchPlayer.Select(x => new MatchPlayerModel { MatchPlayerID = x.MatchPlayerID, Player = x.Player });
        }
    }
}