// DTOs for authentication
export interface LoginDto {
  username: string;
  password: string;
}

export interface RegisterDto {
  username: string;
  password: string;
  role: 'MILITARY' | 'VOLUNTEER';
}

export interface TokenDto {
  token: string;
}

// DTOs for tasks
export interface CreateTaskDto {
  title: string;
  description: string;
  priority: 'LOW' | 'AVERAGE' | 'HIGH';
}

export interface UpdateTaskDto {
  taskId: string;
  title: string;
  description: string;
  priority: 'LOW' | 'AVERAGE' | 'HIGH';
}

export interface ConfirmTaskDto {
  taskId: string;
}

export interface UpdateTaskStatusDto {
  taskId: string;
  status: 'IN_PROGRESS' | 'BLOCKED' | 'DELIVERING' | 'FINISHED';
}

export interface TaskDto {
  id: string;
  militaryId: string;
  volunteerId?: string;
  title: string;
  description: string;
  priority: 'LOW' | 'AVERAGE' | 'HIGH';
  status: 'CREATED' | 'IN_PROGRESS' | 'BLOCKED' | 'DELIVERING' | 'FINISHED' | 'CONFIRMED';
  createdAt: Date;
  startedAt?: Date;
  finishedAt?: Date;
  confirmedAt?: Date;
}

export interface TaskFilterDto {
  status?: string;
  priority?: string;
  militaryId?: string;
  volunteerId?: string;
  createdAtFrom?: Date;
  createdAtTo?: Date;
}

export interface TaskSortingDto {
  sortBy: 'Priority' | 'CreatedAt';
  isDescending: boolean;
}

export interface PaginationDto {
  pageNumber: number;
  pageSize: number;
}

// DTOs for bindings
export interface CreateBindingDto {
  email: string;
}

export interface UpdateBindingDto {
  email: string;
}

export interface BindingDto {
  identityId: string;
  email: string;
}

// User interface
export interface User {
  id: string;
  username: string;
  role: 'MILITARY' | 'VOLUNTEER';
}