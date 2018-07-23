package com.ramanco.samandroid.dialogs;

import android.app.Activity;
import android.content.Context;
import android.support.annotation.NonNull;
import android.support.v7.app.AlertDialog;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.LinearLayout;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.api.dtos.ObitDto;
import com.ramanco.samandroid.api.dtos.ObitHoldingDto;
import com.ramanco.samandroid.enums.ObitType;
import com.ramanco.samandroid.fragments.SendConsolationFragment;
import com.ramanco.samandroid.utils.EnumUtil;
import com.ramanco.samandroid.utils.ExceptionManager;
import com.ramanco.samandroid.utils.PersianDateConverter;

import org.joda.time.DateTime;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.List;
import java.util.Locale;

public class OtherObitsAlertDialog extends AlertDialog {

    public OtherObitsAlertDialog(@NonNull final Context context, ObitDto[] obits, final SendConsolationFragment parentView) {
        super(context);

        // region create alert view:
        LayoutInflater inflater = ((Activity)context).getLayoutInflater();
        View alertView = inflater.inflate(R.layout.alert_related_obits_selection, null);
        //region add checkboxes:
        final LinearLayout choicesView = (LinearLayout) alertView.findViewById(R.id.choices);
        for (ObitDto obit : obits) {
            ObitHoldingDto h = getNearestHolding(obit.getObitHoldings());
            String obitDateString = "";
            if (h != null) {
                PersianDateConverter datePersian = new PersianDateConverter(h.getBeginTimeObject().getYear(), h.getBeginTimeObject().getMonthOfYear(), h.getBeginTimeObject().getDayOfMonth());
                obitDateString = String.format("(%s %s %s)", datePersian.getIranianDate(),
                        context.getResources().getString(R.string.hour),
                        String.format(Locale.getDefault(), "%02d:%02d", h.getBeginTimeObject().getHourOfDay(), h.getBeginTimeObject().getMinuteOfHour()));
            }

            CheckBox checkBox = new CheckBox(context);
            checkBox.setTag(obit.getId());
            checkBox.setText(String.format("%s %s", EnumUtil.getObitTypeText(context, ObitType.valueOf(obit.getObitType())),
                    obitDateString));
            checkBox.setPadding(2, 2, 2, 2);
            choicesView.addView(checkBox);
        }
        //endregion
        //region set on next click listener:
        Button btnNext = (Button) alertView.findViewById(R.id.btn_next);
        btnNext.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                try {
                    List<Integer> selectedObitIds = new ArrayList<>();
                    int childCount = choicesView.getChildCount();
                    for (int i = 0; i < childCount; i++) {
                        CheckBox ch = (CheckBox) choicesView.getChildAt(i);
                        if (ch.isChecked())
                            selectedObitIds.add((Integer) ch.getTag());
                    }
                    String joinedIds = concatIds(selectedObitIds);
                    parentView.setOtherObits(joinedIds);
                    parentView.showTemplateSelectionStep();
                    dismiss();
                } catch (Exception ex) {
                    ExceptionManager.handle(context, ex);
                }
            }
        });
        //endregion
        setView(alertView);
        //endregion

    }

    private String concatIds(List<Integer> ids) {
        Collections.sort(ids);
        StringBuilder str = new StringBuilder();
        for (int i = 0; i < ids.size(); i++) {
            str.append(i == 0 ? Integer.toString(ids.get(i)) : String.format(", %s", Integer.toString(ids.get(i))));
        }
        return str.toString();
    }

    private ObitHoldingDto getNearestHolding(ObitHoldingDto[] holdings) {

        if (holdings == null || holdings.length == 0)
            return null;

        Arrays.sort(holdings);
        ObitHoldingDto nearestHolding = null;
        for (ObitHoldingDto h : holdings) {
            if (h.getEndTimeObject().compareTo(DateTime.now()) > 0)
                nearestHolding = h;
        }

        return nearestHolding;
    }

}
