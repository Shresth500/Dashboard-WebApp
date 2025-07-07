export interface IRegister {
  username: string;
  email: string;
  password: string;
}

export interface ILogin {
  email: string;
  password: string;
}

export interface IAddproduct {
  name: string;
  status: 'Approved' | 'Pending' | 'Rejected';
}

export interface IProducts {
  id: number;
  name: string;
  status: 'Approved' | 'Pending' | 'Rejected';
}

export interface StatusSummaryDTO {
  Accepted: number;
  Pending: number;
  Rejected: number;
}

export interface IProductListItems {
  pageSize: number;
  pageNumber: number;
  totalPages: number;
  products: IProducts[];
  totalItems: number;
}

export interface ILoginByUserName {
  username: string;
  password: string;
}
