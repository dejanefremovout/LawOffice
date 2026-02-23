/**
 * Opposing Party Model
 */
  
export interface OpposingParty {
  id: string;
  officeId: string;
  firstName: string;
  lastName: string;
  address: string | null;
  description: string | null;
  phone: string | null;
  identificationNumber: string | null;
}
