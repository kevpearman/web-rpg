import React, { useState, useEffect } from 'react';
import { gameService, Player } from '../services/gameService';

const GameDashboard: React.FC = () => {
  const [player, setPlayer] = useState<Player | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [apiStatus, setApiStatus] = useState<string>('');

  useEffect(() => {
    testConnection();
  }, []);

  const testConnection = async () => {
    try {
      setLoading(true);
      
      // Test API connection first
      console.log('Testing API connection...');
      const healthCheck = await gameService.healthCheck();
      setApiStatus(`API Status: ${healthCheck}`);
      console.log('Health check response:', healthCheck);

      // Get all players
      console.log('Getting all players...');
      const players = await gameService.getAllPlayers();
      console.log('Players found:', players);

      if (players.length === 0) {
        console.log('No players found, creating test data...');
        await gameService.createTestData();
        
        // Get players again after creating test data
        const newPlayers = await gameService.getAllPlayers();
        console.log('Players after test data creation:', newPlayers);
        
        if (newPlayers.length > 0) {
          const playerWithDetails = await gameService.getPlayer(newPlayers[0].id);
          setPlayer(playerWithDetails);
        }
      } else {
        // Get the first player with full details
        const playerWithDetails = await gameService.getPlayer(players[0].id);
        setPlayer(playerWithDetails);
      }

    } catch (err) {
      console.error('Connection error:', err);
      setError(`Failed to connect to API: ${(err as Error).message}`);
    } finally {
      setLoading(false);
    }
  };

  const createNewPlayer = async () => {
    try {
      setLoading(true);
      const newPlayer = await gameService.createPlayer({
        username: `Player${Date.now()}`,
        email: `player${Date.now()}@test.com`,
        password: 'password123'
      });
      
      const playerWithDetails = await gameService.getPlayer(newPlayer.id);
      setPlayer(playerWithDetails);
    } catch (err) {
      setError(`Failed to create player: ${(err as Error).message}`);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div style={{ padding: '20px', textAlign: 'center' }}>
        <h2>🏪 Merchant RPG</h2>
        <p>Loading... Connecting to backend API...</p>
        {apiStatus && <p style={{ color: 'green' }}>{apiStatus}</p>}
      </div>
    );
  }

  if (error) {
    return (
      <div style={{ padding: '20px', textAlign: 'center' }}>
        <h2>🏪 Merchant RPG</h2>
        <div style={{ color: 'red', marginBottom: '20px' }}>
          <strong>Connection Error:</strong>
          <p>{error}</p>
        </div>
        <button onClick={testConnection} style={{ padding: '10px 20px' }}>
          Retry Connection
        </button>
        <div style={{ marginTop: '20px', fontSize: '14px', color: '#666' }}>
          <p>Make sure your backend API is running on https://localhost:7050</p>
          <p>Check the browser console (F12) for more details</p>
        </div>
      </div>
    );
  }

  return (
    <div style={{ padding: '20px', maxWidth: '800px', margin: '0 auto' }}>
      <h1>🏪 Merchant RPG</h1>
      
      <div style={{ marginBottom: '20px', padding: '15px', backgroundColor: '#f0f0f0', borderRadius: '5px' }}>
        <p style={{ color: 'green', margin: 0 }}>{apiStatus}</p>
      </div>

      {player ? (
        <div>
          <h2>Welcome, {player.username}!</h2>
          
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '15px', marginBottom: '20px' }}>
            <div style={{ padding: '15px', backgroundColor: '#e8f5e8', borderRadius: '5px' }}>
              <h3>💰 Gold</h3>
              <p style={{ fontSize: '24px', margin: 0 }}>{player.gold}</p>
            </div>
            
            <div style={{ padding: '15px', backgroundColor: '#e8f0ff', borderRadius: '5px' }}>
              <h3>⚔️ Adventurers</h3>
              <p style={{ fontSize: '24px', margin: 0 }}>{player.adventurers?.length || 0}</p>
            </div>
          </div>

          {player.adventurers && player.adventurers.length > 0 && (
            <div>
              <h3>Your Adventurers:</h3>
              <div style={{ display: 'grid', gap: '10px' }}>
                {player.adventurers.map(adventurer => (
                  <div key={adventurer.id} style={{ 
                    padding: '15px', 
                    backgroundColor: adventurer.isOnMission ? '#ffe8e8' : '#f9f9f9', 
                    borderRadius: '5px',
                    border: '1px solid #ddd'
                  }}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                      <div>
                        <strong>{adventurer.name}</strong> (Level {adventurer.level})
                        <br />
                        <small>
                          STR: {adventurer.strength} | AGI: {adventurer.agility} | INT: {adventurer.intelligence}
                        </small>
                      </div>
                      <div style={{ textAlign: 'right' }}>
                        <div>EXP: {adventurer.experience}</div>
                        <div style={{ color: adventurer.isOnMission ? 'red' : 'green' }}>
                          {adventurer.isOnMission ? '🗺️ On Mission' : '✅ Available'}
                        </div>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>
      ) : (
        <div style={{ textAlign: 'center' }}>
          <h2>No Player Found</h2>
          <button onClick={createNewPlayer} style={{ padding: '10px 20px', fontSize: '16px' }}>
            Create New Player
          </button>
        </div>
      )}

      <div style={{ marginTop: '30px', textAlign: 'center' }}>
        <button onClick={testConnection} style={{ padding: '10px 20px', marginRight: '10px' }}>
          Refresh Data
        </button>
        <button onClick={createNewPlayer} style={{ padding: '10px 20px' }}>
          Create Another Player
        </button>
      </div>
    </div>
  );
};

export default GameDashboard;