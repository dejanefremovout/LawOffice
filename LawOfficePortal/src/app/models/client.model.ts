/**
 * Client Model
 */

export interface Client {
  id: string;
  firstName: string;
  lastName: string;
  address: string | null;
  description: string | null;
  phone: string | null;
  identificationNumber: string | null;
}
