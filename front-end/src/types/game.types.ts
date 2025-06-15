export interface Player {
  id: number;
  username: string;
  email: string;
  gold: number;
  createdAt: string;
  lastActive: string;
  adventurers?: Adventurer[];
  resources?: PlayerResource[];
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
  createdAt: string;
  currentMission?: Mission;
}

export interface Mission {
  id: number;
  adventurerId: number;
  mapId: string;
  startTime: string;
  duration: number; // in seconds
  status: 'active' | 'completed' | 'failed';
  rewards?: MissionReward;
  createdAt: string;
  adventurer?: Adventurer;
  gameMap?: GameMap;
}

export interface PlayerResource {
  id: number;
  playerId: number;
  resourceType: string;
  quantity: number;
}

export interface GameMap {
  id: string;
  name: string;
  difficulty: 'easy' | 'medium' | 'hard' | 'boss';
  requiredLevel: number;
  duration: number; // in seconds
  rewardRanges: RewardRange;
  unlocksMapId?: string;
}

export interface RewardRange {
  goldMin: number;
  goldMax: number;
  experienceMin: number;
  experienceMax: number;
  resources: ResourceDrop[];
}

export interface ResourceDrop {
  type: string;
  minQuantity: number;
  maxQuantity: number;
  dropChance: number; // 0-1 (0-100%)
}

export interface MissionReward {
  gold: number;
  experience: number;
  resources: ResourceReward[];
}

export interface ResourceReward {
  type: string;
  quantity: number;
}

// API request/response types
export interface CreatePlayerRequest {
  username: string;
  email: string;
  password: string;
}

export interface StartMissionRequest {
  adventurerId: number;
  mapId: string;
}

export interface MissionProgress {
  missionId: number;
  phase: 'traveling_to' | 'fighting' | 'traveling_back';
  timeRemaining: number;
  totalDuration: number;
  progressPercentage: number;
}

// UI-specific types
export interface MissionStatus {
  mission: Mission;
  progress: MissionProgress;
  isCompleted: boolean;
  timeUntilComplete: number;
}

export interface AdventurerStats {
  totalStats: number;
  combatPower: number;
  averageLevel: number;
}

export interface PlayerStats {
  totalGold: number;
  totalAdventurers: number;
  activeMissions: number;
  totalResources: number;
  highestAdventurerLevel: number;
}