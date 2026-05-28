"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.getOfficeId = getOfficeId;
function getOfficeId(req) {
    const officeId = req.headers.get('x-office-id');
    if (!officeId?.trim()) {
        throw new Error('Office Id header is required.');
    }
    return officeId.trim();
}
//# sourceMappingURL=officeIdHelper.js.map