"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ServiceBusOpposingPartyDeletedPublisher = void 0;
class ServiceBusOpposingPartyDeletedPublisher {
    sender;
    constructor(sender) {
        this.sender = sender;
        if (!sender)
            throw new Error('ServiceBusSender is required.');
    }
    async publish(message, cancellationToken) {
        const sbMessage = {
            body: JSON.stringify(message),
            contentType: 'application/json',
            messageId: crypto.randomUUID(),
            subject: 'OpposingPartyDeleted',
            applicationProperties: {
                officeId: message.officeId,
            },
        };
        await this.sender.sendMessages(sbMessage, { abortSignal: cancellationToken });
    }
}
exports.ServiceBusOpposingPartyDeletedPublisher = ServiceBusOpposingPartyDeletedPublisher;
//# sourceMappingURL=serviceBusOpposingPartyDeletedPublisher.js.map