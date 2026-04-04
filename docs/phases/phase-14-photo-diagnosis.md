# Phase 14: Plant Health Photo Diagnosis

**Status:** ✅ Complete
**Branch:** `feature/phase-14-photo-diagnosis` (merged to main)
**Tests added:** 3 (`PlantAdvisorServiceTests` — DiagnosePlantAsync cases)
**Total tests after phase:** 96

---

## Goal

Allow users to photograph their plant inside the Ask Claude chat and receive a full health diagnosis from Claude — covering pests, disease, overwatering/underwatering, and nutrient deficiencies.

---

## Decisions made

1. **Where**: Camera and gallery buttons inside the existing Ask Claude chat (`PlantAdvisorPage`), not a standalone page. Integrates naturally into the conversation flow.
2. **Scope**: Full assessment — health issues (pests, disease, overwatering, nutrient deficiency) plus specific care recommendations.

---

## What was built

### Core (PlantPal.Core)

| File | Change |
|---|---|
| `Models/PhotoResult.cs` | New — holds `byte[] Bytes` + `string MimeType` returned by `IMediaPickerService` |
| `Interfaces/IMediaPickerService.cs` | New — `PickPhotoAsync()` and `CapturePhotoAsync()`, both return `PhotoResult?` (null on cancel/deny) |
| `Interfaces/IPlantAdvisorService.cs` | Added `DiagnosePlantAsync(species, question, imageBytes, mimeType, context)` |
| `Services/PlantAdvisorService.cs` | Implemented `DiagnosePlantAsync` and `BuildVisionRequestBody` — sends base64 image block + text block as mixed-content Anthropic message |
| `ViewModels/PlantAdvisorViewModel.cs` | Added `AttachedImageBytes`, `HasAttachedPhoto`, `AttachPhotoFromGalleryCommand`, `AttachPhotoFromCameraCommand`, `RemovePhotoCommand`; `SendAsync` routes to vision when photo is attached |

### MAUI (PlantPal)

| File | Change |
|---|---|
| `Services/MediaPickerService.cs` | New — MAUI concrete using `MediaPicker.PickPhotoAsync()` / `CapturePhotoAsync()`; infers MIME type from file extension; returns `null` on any failure |
| `Pages/PlantAdvisorPage.xaml` | Added 🖼 gallery + 📷 camera buttons to the input bar; photo preview strip with ✕ remove button above input |
| `Pages/PlantAdvisorPage.xaml.cs` | `OnViewModelPropertyChanged` — updates `AttachedPhotoPreview.Source` from bytes via `ImageSource.FromStream` |
| `MauiProgram.cs` | Registered `IMediaPickerService` → `MediaPickerService` as singleton |

---

## Technical approach

### Anthropic vision API

The existing `IHttpClientWrapper.PostStringAsync` is reused — only the JSON body differs. Vision messages use an array for `content` instead of a plain string:

```json
{
  "role": "user",
  "content": [
    {
      "type": "image",
      "source": { "type": "base64", "media_type": "image/jpeg", "data": "..." }
    },
    {
      "type": "text",
      "text": "What is wrong with my plant?"
    }
  ]
}
```

The system prompt for vision calls is specialist-framed:
> "You are a plant health expert. The user is sharing a photo of their {species}. Provide a comprehensive diagnosis: identify any visible issues such as pests, disease, overwatering, underwatering, or nutrient deficiencies. Give specific actionable care recommendations."

### Chat history storage

Photos are not stored in SQLite — they can be several MB each. The user message is stored as:
- With typed question: `📷 What is this white powder?`
- Without question: `📷 Photo sent for diagnosis`

### Permission handling

`MediaPickerService` wraps all calls in try/catch and returns `null` on any failure (permission denied, cancelled, no camera). The ViewModel silently ignores null — no error is shown, which matches the existing permission strategy (never crash, treat denial as valid state).

---

## Files NOT changed

- `IPlantRepository`, `IWateringLogRepository`, `DatabaseService`, `WateringLogRepository` — photo data is never persisted
- `AdvisorMessage` model — no schema change needed
- All other pages — the feature is self-contained within `PlantAdvisorPage`

---

## Success condition

- `./scripts/test-all.sh` — 96/96 green ✅
- App tested: tap 📷 in Ask Claude → photo selected → diagnosis received from Claude

## Deviations from original plan

None — implemented exactly as discussed.
