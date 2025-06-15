import axios from 'axios';

// Configure the API base URL to match your running backend
const API_BASE_URL = 'https://localhost:7050/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Simple interfaces for our data
export interface Player {
  id: number;
  username: string;
  email: string;
  gold: number;
  createdAt: string;
  lastActive: string;
  adventurers?: Adventurer[];
}

export interface Adventurer {
  id: number;
  playerId: number;
  name: string;
  level: number;
  strength: number;
  agility: number;
  intelligence: number;
  experience: number;
  isOnMission: boolean;
}

export interface CreatePlayerRequest {
  username: string;
  email: string;
  password: string;
}

// Game service functions
export const gameService = {
  // Test API connection
  async healthCheck(): Promise<string> {
    const response = await api.get('/health');
    return response.data;
  },

  // Get all players
  async getAllPlayers(): Promise<Player[]> {
    const response = await api.get('/players');
    return response.data;
  },

  // Get specific player with adventurers
  async getPlayer(id: number): Promise<Player> {
    const response = await api.get(`/players/${id}`);
    return response.data;
  },

  // Create new player
  async createPlayer(request: CreatePlayerRequest): Promise<Player> {
    const response = await api.post('/players', request);
    return response.data;
  },

  // Create test data (useful for development)
  async createTestData(): Promise<void> {
    await api.post('/players/test-data');
  }
};