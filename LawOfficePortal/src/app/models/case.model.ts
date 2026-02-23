/**
 * Case Model
 */
export interface Case {
  id: string;
  officeId: string;
  clientIds: string[];
  opposingPartyIds: string[];
  identificationNumber: string;
  description: string | null;
  active: boolean;
  competentCourt: string | null;
  city: string | null;
  year: number | null;
  judge: string | null;
}
