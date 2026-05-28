"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Party = void 0;
function requireNonBlank(value, fieldName) {
    if (!value || !value.trim()) {
        throw new Error(`${fieldName} is required and cannot be blank.`);
    }
    return value.trim();
}
class Party {
    _id;
    _officeId;
    _firstName;
    _lastName;
    _address;
    _description;
    _phone;
    _identificationNumber;
    constructor(id, officeId, firstName, lastName, address, description, phone, identificationNumber) {
        this._id = requireNonBlank(id, 'id');
        this._officeId = requireNonBlank(officeId, 'officeId');
        this._firstName = requireNonBlank(firstName, 'firstName');
        this._lastName = requireNonBlank(lastName, 'lastName');
        this._address = address?.trim() || undefined;
        this._description = description?.trim() || undefined;
        this._phone = phone?.trim() || undefined;
        this._identificationNumber = identificationNumber?.trim() || undefined;
    }
    static create(officeId, firstName, lastName, address, description, phone, identificationNumber) {
        return new Party(crypto.randomUUID(), officeId, firstName, lastName, address, description, phone, identificationNumber);
    }
    get id() {
        return this._id;
    }
    get officeId() {
        return this._officeId;
    }
    get firstName() {
        return this._firstName;
    }
    get lastName() {
        return this._lastName;
    }
    get address() {
        return this._address;
    }
    get description() {
        return this._description;
    }
    get phone() {
        return this._phone;
    }
    get identificationNumber() {
        return this._identificationNumber;
    }
    setName(firstName, lastName) {
        this._firstName = requireNonBlank(firstName, 'firstName');
        this._lastName = requireNonBlank(lastName, 'lastName');
    }
    setAddress(address) {
        this._address = address?.trim() || undefined;
    }
    setDescription(description) {
        this._description = description?.trim() || undefined;
    }
    setPhone(phone) {
        this._phone = phone?.trim() || undefined;
    }
    setIdentificationNumber(identificationNumber) {
        this._identificationNumber = identificationNumber?.trim() || undefined;
    }
}
exports.Party = Party;
//# sourceMappingURL=party.js.map