export declare class Party {
    private readonly _id;
    private readonly _officeId;
    private _firstName;
    private _lastName;
    private _address;
    private _description;
    private _phone;
    private _identificationNumber;
    constructor(id: string, officeId: string, firstName: string, lastName: string, address: string | undefined, description: string | undefined, phone: string | undefined, identificationNumber: string | undefined);
    static create(officeId: string, firstName: string, lastName: string, address: string | undefined, description: string | undefined, phone: string | undefined, identificationNumber: string | undefined): Party;
    get id(): string;
    get officeId(): string;
    get firstName(): string;
    get lastName(): string;
    get address(): string | undefined;
    get description(): string | undefined;
    get phone(): string | undefined;
    get identificationNumber(): string | undefined;
    setName(firstName: string, lastName: string): void;
    setAddress(address: string | undefined): void;
    setDescription(description: string | undefined): void;
    setPhone(phone: string | undefined): void;
    setIdentificationNumber(identificationNumber: string | undefined): void;
}
//# sourceMappingURL=party.d.ts.map